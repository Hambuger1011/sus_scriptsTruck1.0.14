﻿/*
 Desc: 一个可以运行时 Hook Mono 方法的工具，让你可以无需修改 UnityEditor.dll 等文件就可以重写其函数功能
 Author: Misaka Mikoto
 Github: https://github.com/easy66/MonoHooker
 */

#if UNITY_EDITOR
using System;
using System.Reflection;


/*
>>>>>>> 原始 UnityEditor.LogEntries.Clear 一型(.net 4.x)
0000000000403A00 < | 55                                 | push rbp                                     |
0000000000403A01   | 48 8B EC                           | mov rbp,rsp                                  |
0000000000403A04   | 48 81 EC 80 00 00 00               | sub rsp,80                                   |
0000000000403A0B   | 48 89 65 B0                        | mov qword ptr ss:[rbp-50],rsp                |
0000000000403A0F   | 48 89 6D A8                        | mov qword ptr ss:[rbp-58],rbp                |
0000000000403A13   | 48 89 5D C8                        | mov qword ptr ss:[rbp-38],rbx                | <<
0000000000403A17   | 48 89 75 D0                        | mov qword ptr ss:[rbp-30],rsi                |
0000000000403A1B   | 48 89 7D D8                        | mov qword ptr ss:[rbp-28],rdi                |
0000000000403A1F   | 4C 89 65 E0                        | mov qword ptr ss:[rbp-20],r12                |
0000000000403A23   | 4C 89 6D E8                        | mov qword ptr ss:[rbp-18],r13                |
0000000000403A27   | 4C 89 75 F0                        | mov qword ptr ss:[rbp-10],r14                |
0000000000403A2B   | 4C 89 7D F8                        | mov qword ptr ss:[rbp-8],r15                 |
0000000000403A2F   | 49 BB 00 2D 1E 1A FE 7F 00 00      | mov r11,7FFE1A1E2D00                         |
0000000000403A39   | 4C 89 5D B8                        | mov qword ptr ss:[rbp-48],r11                |
0000000000403A3D   | 49 BB 08 2D 1E 1A FE 7F 00 00      | mov r11,7FFE1A1E2D08                         |


>>>>>>> 二型(.net 2.x)
0000000000403E8F   | 55                                 | push rbp                                     |
0000000000403E90   | 48 8B EC                           | mov rbp,rsp                                  |
0000000000403E93   | 48 83 EC 70                        | sub rsp,70                                   |
0000000000403E97   | 48 89 65 C8                        | mov qword ptr ss:[rbp-38],rsp                |
0000000000403E9B   | 48 89 5D B8                        | mov qword ptr ss:[rbp-48],rbx                |
0000000000403E9F   | 48 89 6D C0                        | mov qword ptr ss:[rbp-40],rbp                | <<(16)
0000000000403EA3   | 48 89 75 F8                        | mov qword ptr ss:[rbp-8],rsi                 |
0000000000403EA7   | 48 89 7D F0                        | mov qword ptr ss:[rbp-10],rdi                |
0000000000403EAB   | 4C 89 65 D0                        | mov qword ptr ss:[rbp-30],r12                |
0000000000403EAF   | 4C 89 6D D8                        | mov qword ptr ss:[rbp-28],r13                |
0000000000403EB3   | 4C 89 75 E0                        | mov qword ptr ss:[rbp-20],r14                |
0000000000403EB7   | 4C 89 7D E8                        | mov qword ptr ss:[rbp-18],r15                |
0000000000403EBB   | 48 83 EC 20                        | sub rsp,20                                   |
0000000000403EBF   | 49 BB 18 3F 15 13 FE 7F 00 00      | mov r11,7FFE13153F18                         |
0000000000403EC9   | 41 FF D3                           | call r11                                     |
0000000000403ECC   | 48 83 C4 20                        | add rsp,20                                   |

 */


/// <summary>
/// Hook 类，用来 Hook 某个 C# 方法
/// </summary>
public unsafe class MethodHooker
{
    public bool isHooked { get; private set; }

    private MethodInfo  _targetMethod;       // 需要被hook的目标方法
    private MethodInfo  _replacementMethod;  // 被hook后的替代方法
    private MethodInfo  _proxyMethod;        // 目标方法的代理方法(可以通过此方法调用被hook后的原方法)

    private IntPtr      _targetPtr;          // 目标方法被 jit 后的地址指针
    private IntPtr      _replacementPtr;
    private IntPtr      _proxyPtr;

    private static readonly byte[] s_jmpBuff = new byte[]
    {
        0xFF, 0x25, 0x00, 0x00, 0x00, 0x00,         // jmp [rip]
        0x00, 0x00,0x00,0x00,0x00,0x00,0x00,0x00,   // $val
    };


    private byte[]      _jmpBuff;
    private byte[]      _proxyBuff;

    /// <summary>
    /// 创建一个 Hooker
    /// </summary>
    /// <param name="targetMethod">需要替换的目标方法</param>
    /// <param name="replacementMethod">准备好的替换方法</param>
    /// <param name="proxyMethod">如果还需要调用原始目标方法，可以通过此参数的方法调用，如果不需要可以填 null</param>
    public MethodHooker(MethodInfo targetMethod, MethodInfo replacementMethod, MethodInfo proxyMethod = null)
    {
        _targetMethod       = targetMethod;
        _replacementMethod  = replacementMethod;
        _proxyMethod        = proxyMethod;

        _targetPtr      = _targetMethod.MethodHandle.GetFunctionPointer();
        _replacementPtr = _replacementMethod.MethodHandle.GetFunctionPointer();
        if(proxyMethod != null)
            _proxyPtr       = _proxyMethod.MethodHandle.GetFunctionPointer();

        _jmpBuff = new byte[s_jmpBuff.Length];
    }

    public void Install()
    {
        if (isHooked)
            return;

        HookerPool.AddHooker(_targetMethod, this);

        InitProxyBuff();
        BackupHeader();
        PatchTargetMethod();
        PatchProxyMethod();

        isHooked = true;
    }

    public void Uninstall()
    {
        if (!isHooked)
            return;

        byte* pTarget = (byte*)_targetPtr.ToPointer();
        for (int i = 0; i < _proxyBuff.Length; i++)
            *pTarget++ = _proxyBuff[i];

        isHooked = false;
        HookerPool.RemoveHooker(_targetMethod);
    }

    #region private
    /// <summary>
    ///  根据具体指令填充 ProxyBuff
    /// </summary>
    /// <returns></returns>
    private void InitProxyBuff()
    {
        byte* pTarget = (byte*)_targetPtr.ToPointer();

        uint requireSize = DotNetDetour.LDasm.SizeofMinNumByte(pTarget, s_jmpBuff.Length);
        _proxyBuff = new byte[requireSize];
    }

    /// <summary>
    /// 备份原始方法头
    /// </summary>
    private void BackupHeader()
    {
        byte* pTarget = (byte*)_targetPtr.ToPointer();
        for (int i = 0; i < _proxyBuff.Length; i++)
            _proxyBuff[i] = *pTarget++;
    }

    // 将原始方法跳转到我们的方法
    private void PatchTargetMethod()
    {
        Array.Copy(s_jmpBuff, _jmpBuff, _jmpBuff.Length);
        fixed (byte* p = &_jmpBuff[6])
        {
            *((ulong*)p) = (ulong)_replacementPtr.ToInt64();
        }

        byte* pTarget = (byte*)_targetPtr.ToPointer();
        for (int i = 0; i < _jmpBuff.Length; i++)
            *pTarget++ = _jmpBuff[i];
    }

    /// <summary>
    /// 让 Proxy 方法的功能变成跳转向原始方法
    /// </summary>
    private void PatchProxyMethod()
    {
        if (_proxyPtr == IntPtr.Zero)
            return;

        byte* pProxy = (byte*)_proxyPtr.ToPointer();
        for (int i = 0; i < _proxyBuff.Length; i++)     // 先填充头
            *pProxy++ = _proxyBuff[i];

        fixed (byte* p = &_jmpBuff[6])                  // 将跳转指向原函数跳过头的位置
        {
            *((ulong*)p) = (ulong)(_targetPtr.ToInt64() + _proxyBuff.Length);
        }

        for (int i = 0; i < _jmpBuff.Length; i++)       // 再填充跳转
            *pProxy++ = _jmpBuff[i];
    }

    #endregion
}
#endif