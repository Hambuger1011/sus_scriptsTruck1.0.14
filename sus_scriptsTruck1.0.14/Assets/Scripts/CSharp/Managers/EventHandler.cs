using System;
using System.Collections.Generic;
using XLua;

[CSharpCallLua]

public delegate void VoidEventHandler();

[CSharpCallLua]

public delegate void EventHandler(object sender);

[CSharpCallLua]

public delegate void DataEventHandler(object sender, object data);

[CSharpCallLua]

public delegate void DataTypeEventHandler(string type, object sender, object data);

[CSharpCallLua]

public delegate void DataChangeHandler(object data);

[CSharpCallLua]

public delegate void TypeEventHandler(string type, object data);

[CSharpCallLua]

public delegate void CloseEventHandler(uint detail);
