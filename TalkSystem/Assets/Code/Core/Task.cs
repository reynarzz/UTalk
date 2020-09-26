using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace TalkSystem
{
    public class Task : MonoBehaviour
    {
        private void Awake()
        {
            //Debug.Log("Awake");
        }

        private void OnEnable()
        {
            //Debug.Log("Enabled");
        }

        private List<string> _s;


        private void Start()
        {
            //var e = new Enums<string>();
            //e.Add("Hello");
            //e.Add("It's me");

            //var a = new Alloc<string>();
            //a.Add("Hello");
            //a.Add("Alloc!!");
            //_s = new List<string>();

            //foreach (var item in a)
            //{
            //    //Debug.Log(item);
            //}

            //Debug.Log("Start");

        }

        private struct A : IInterface
        {

        }

        private interface IInterface
        {

        }

        private void FixedUpdate()
        {
            //Debug.Log("FixedUpdate realTime: " + Time.realtimeSinceStartup);
        }

        private void Update()
        {
            IInterface a = new A();

            //_s.Add("asdaklsd");
            //GC.Collect();

            //if (_s.Count > 400)
            //{
            //    _s.Clear();

            //}
            //_s.RemoveAt(0);

            //Concatenate();
            //Debug.Log("Update realTime: " + Time.realtimeSinceStartup);
        }

        private void Concatenate()
        {
            var a = new String(new char[] { 'a', 'b' });

        }

        private void OnDisable()
        {

        }

        private void Reset()
        {

        }
    }


    public class Alloc<T> : IEnumerable, IEnumerator
    {
        private List<T> _list;

        private int _index;

        public object Current => _list.ElementAtOrDefault(_index - 1);

        public Alloc()
        {
            _list = new List<T>();

        }

        public bool MoveNext()
        {
            var canMoveNext = _index < _list.Count;

            if (canMoveNext)
            {
                _index++;
            }

            return canMoveNext;
        }

        public void Reset()
        {
            _index = 0;
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }
    }
    public class Enums<T>
    {

        private int _index;

        public struct Container<T>
        {
            private List<T> _list;

            private int _index;

            public T Current => _list.ElementAtOrDefault(_index - 1);

            public Container(int list) : this()
            {
                //Debug.Log("Called");
                _list = new List<T>();
            }

            public bool MoveNext()
            {
                var canMoveNext = _index < _list.Count;

                if (canMoveNext)
                {
                    _index++;
                }

                return canMoveNext;
            }

            public void Reset()
            {
                _index = 0;
            }

            public void Add(T item)
            {
                _list.Add(item);
            }
        }

        private Container<T> _container;

        public Enums()
        {
            _container = new Container<T>(2);

        }

        public void Add(T item)
        {
            _container.Add(item);
        }

        public Container<T> GetEnumerator()
        {
            return _container;
        }
    }
}
