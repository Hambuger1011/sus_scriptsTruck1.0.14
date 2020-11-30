
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public abstract class UIBookReadingElement : MonoBehaviour
    {
#if NOT_USE_LUA
        public abstract void Bind(BookReadingForm form);

        public abstract void ResetUI();

        public abstract void SetSkin();

        public abstract void Dispose();
#endif
    }
}