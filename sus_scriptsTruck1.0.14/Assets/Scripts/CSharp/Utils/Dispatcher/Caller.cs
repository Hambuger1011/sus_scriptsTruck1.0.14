using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
    /// <summary>
    /// call
    /// </summary>
    public class Caller 
    {
        private Dictionary<string, List<Delegate>> _callerMapOne;

        public Caller()
        {
            _callerMapOne = new Dictionary<string, List<Delegate>>();
        }
    
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public void addCall<T>(string key, Action<T> action)
        {
            if (action == null) return;
            List<Delegate> actionlist = null;
            if (_callerMapOne.TryGetValue(key, out actionlist))
            {
                actionlist.Add(action);
            }
            else
            {
                actionlist = new List<Delegate>();
                actionlist.Add(action);
                _callerMapOne[key] = actionlist;
            }
        }

          public bool call<T>(string key,T t)
          {
              if (t == null) return false;
              List<Delegate> actionlist = null;
              if (_callerMapOne.TryGetValue(key, out actionlist))
              {
                  int len = actionlist.Count;
                  Action<T> oldAction = null;
                  for (int i = 0; i < len; i++)
                  {
                      oldAction = (Action<T>)actionlist[i];
                      oldAction(t);
                  }
                  return true;
              }
              else 
              {
                   return false;
              }
          }
          public void removeAllCall(string key)
          {
              List<Delegate> actionlist = null;
              if (_callerMapOne.TryGetValue(key, out actionlist))
              {
                  _callerMapOne.Remove(key);
              }

          }
         public void removeCall<T>(string key,Action<T> action)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Remove(action);
                 if (actionlist.Count == 0)
                 {
                     _callerMapOne.Remove(key);
                 }
             }
        }


         /// <summary>
         /// 添加
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="action"></param>
         public void addCall(string key, Action action)
         {
             if (action == null) return;
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Add(action);
             }
             else
             {
                 actionlist = new List<Delegate>();
                 actionlist.Add(action);
                 _callerMapOne[key] = actionlist;
             }
            
         }
         public bool call(string key)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 int len = actionlist.Count;
                 Action oldAction = null;
                 for (int i = 0; i < len; i++)
                 {
                     oldAction = (Action)actionlist[i];
                     oldAction();
                 }
                 return true;
             }
             else
             {
                 return false;
             }
         }
         
         public void removeCall(string key, Action action)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Remove(action);
                 if (actionlist.Count == 0)
                 {
                     _callerMapOne.Remove(key);
                 }
             }
         }


        ////==============================================================///////
         /// <summary>
         /// 添加
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="action"></param>
         public void addCall<T,T1>(string key, Action<T,T1> action)
         {
             if (action == null) return;
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Add(action);
             }
             else
             {
                 actionlist = new List<Delegate>();
                 actionlist.Add(action);
                 _callerMapOne[key] = actionlist;
             }
         }

         public bool call<T,T1>(string key, T t,T1 t1)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 int len = actionlist.Count;
                 Action<T, T1> oldAction = null;
                 for (int i = 0; i < len; i++)
                 {
                     oldAction = (Action<T, T1>)actionlist[i];
                     oldAction(t,t1);
                 }
                 return true;
             }
             else
             {
                 return false;
             }
         }

         public void removeCall<T, T1>(string key, Action<T, T1> action)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Remove(action);
                 if (actionlist.Count == 0)
                 {
                     _callerMapOne.Remove(key);
                 }
             }
         }


         ////==================================3============================///////
         /// <summary>
         /// 添加
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="action"></param>
         public void addCall<T, T1, T2>(string key, Action<T, T1, T2> action)
         {
             if (action == null) return;
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Add(action);
             }
             else
             {
                 actionlist = new List<Delegate>();
                 actionlist.Add(action);
                 _callerMapOne[key] = actionlist;
             }
         }

         public bool call<T, T1, T2>(string key, T t, T1 t1, T2 t2)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 int len = actionlist.Count;
                 Action<T, T1, T2> oldAction = null;
                 for (int i = 0; i < len; i++)
                 {
                     oldAction = (Action<T, T1, T2>)actionlist[i];
                     oldAction(t, t1,t2);
                 }
                 return true;
             }
             else
             {
                 return false;
             }
         }

         public void removeCall<T, T1, T2>(string key, Action<T, T1, T2> action)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Remove(action);
                 if (actionlist.Count == 0)
                 {
                     _callerMapOne.Remove(key);
                 }
             }

         }



         ////==================================3============================///////
         /// <summary>
         /// 添加
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="action"></param>
         public void addCall<T, T1, T2, T3>(string key, Action<T, T1, T2, T3> action)
         {
             if (action == null) return;
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Add(action);
             }
             else
             {
                 actionlist = new List<Delegate>();
                 actionlist.Add(action);
                 _callerMapOne[key] = actionlist;
             }
         }

         public bool call<T, T1, T2, T3>(string key, T t, T1 t1, T2 t2, T3 t3)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 int len = actionlist.Count;
                 Action<T, T1, T2,T3> oldAction = null;
                 for (int i = 0; i < len; i++)
                 {
                     oldAction = (Action<T, T1, T2,T3>)actionlist[i];
                     oldAction(t, t1, t2,t3);
                 }
                 return true;
             }
             else
             {
                 return false;
             }
         }

         public void removeCall<T, T1, T2, T3>(string key, Action<T, T1, T2, T3> action)
         {
             List<Delegate> actionlist = null;
             if (_callerMapOne.TryGetValue(key, out actionlist))
             {
                 actionlist.Remove(action);
                 if (actionlist.Count == 0)
                 {
                     _callerMapOne.Remove(key);
                 }
             }
         }
    }
