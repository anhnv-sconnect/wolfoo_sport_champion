using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhNV.GameBase
{
    public interface ILocalSaveLoad<T> where T : class
    {
        public void Save();
        public T Load();
        public void Init(); 
        public void Reset();
        public void Read();
    }
}
