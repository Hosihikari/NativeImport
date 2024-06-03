using Hosihikari.NativeInterop.Generation;
using Hosihikari.NativeInterop.Import;
using Hosihikari.NativeInterop.Unmanaged.STL.Interface;
using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Hosihikari.NativeInterop.Unmanaged.STL;

[StructLayout(LayoutKind.Sequential)]
[PredefinedType(TypeName = "class std::map", IgnoreTemplateArgs = true)]
[SupportedOSPlatform("windows")]
public unsafe struct StdMap
{
    public void* head;
    public ulong size;
}

[SupportedOSPlatform("windows")]
public sealed unsafe class StdMap<TKey, TValue>(nint ptr, bool ownsInstance, bool ownsMemory) :
    ICppInstance<StdMap<TKey, TValue>>,
    IStdIterable<StdMap<TKey, TValue>.Iterator, StdPair<TKey, TValue>>,
    IEnumerable<StdPair<TKey, TValue>>
    /*IMoveableCppInstance<StdMap<TKey, TValue>>,
    ICopyableCppInstance<StdMap<TKey, TValue>>*/
    where TKey : unmanaged, IComparisonOperators<TKey, TKey, bool>
    where TValue : unmanaged
{
    private bool disposedValue;

    [StructLayout(LayoutKind.Sequential)]
    public struct _Tree_node
    {

        public enum _Tree_color : byte
        {
            Red = 0,
            Black = 1
        }

        public _Tree_node* _Left;
        public _Tree_node* _Parent;
        public _Tree_node* _Right;
        public _Tree_color _Color;
        public NativeBoolean _IsNil;
        public StdPair<TKey, TValue> _Myval;
    }

    public struct Iterator(_Tree_node* ptr) : IStdIterator<Iterator, StdPair<TKey, TValue>>
    {
        private _Tree_node* _Ptr = ptr;

        public ref StdPair<TKey, TValue> Target => ref _Ptr->_Myval;

        public readonly override bool Equals(object? obj) => obj is Iterator iterator && _Ptr == iterator._Ptr;

        public readonly override int GetHashCode()
        {
            return (int)_Ptr;
        }

        public static Iterator operator ++(Iterator value)
        {
            var _Ptr = value._Ptr;

            if (_Ptr->_Right->_IsNil)
            {
                _Tree_node* _Pnode;
                while (!(_Pnode = _Ptr->_Parent)->_IsNil && _Ptr == _Pnode->_Right)
                    _Ptr = _Pnode;

                _Ptr = _Pnode;
            }
            else
                _Ptr = _Tree_val._Min(_Ptr->_Right);

            return new(_Ptr);
        }

        public static Iterator operator --(Iterator value)
        {
            var _Ptr = value._Ptr;

            if (_Ptr->_IsNil)
                _Ptr = _Ptr->_Right;
            else if (_Ptr->_Left->_IsNil)
            {
                _Tree_node* _Pnode;
                while (!(_Pnode = _Ptr->_Parent)->_IsNil && _Ptr == _Pnode->_Left)
                    _Ptr = _Pnode;

                if (!_Ptr->_IsNil)
                    _Ptr = _Pnode;
            }
            else
                _Ptr = _Tree_val._Max(_Ptr->_Left);

            return new(_Ptr);
        }

        public static bool operator ==(Iterator left, Iterator right) => left._Ptr == right._Ptr;

        public static bool operator !=(Iterator left, Iterator right) => left._Ptr != right._Ptr;
    }

    public static ulong ClassSize => 16;

    public nint Pointer { get; set; } = ptr;
    public bool OwnsInstance { get; set; } = ownsInstance;
    public bool OwnsMemory { get; set; } = ownsMemory;

    private _Tree_val* _This => (_Tree_val*)Pointer.ToPointer();

    public static StdMap<TKey, TValue> ConstructInstance(nint ptr, bool owns, bool ownsMemory) => new(ptr, owns, ownsMemory);

    static object ICppInstanceNonGeneric.ConstructInstance(nint ptr, bool owns, bool ownsMemory) => ConstructInstance(ptr, owns, ownsMemory);

    public void Destruct() => DestructInstance(this);

    public static implicit operator nint(StdMap<TKey, TValue> ins)
    {
        throw new NotImplementedException();
    }

    public static implicit operator void*(StdMap<TKey, TValue> ins)
    {
        throw new NotImplementedException();
    }

    private void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (OwnsInstance)
        {
            Destruct();
            LibNative.operator_delete(this);
        }

        disposedValue = true;
    }

    ~StdMap() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public struct Enumerator(StdMap<TKey, TValue> ins) : IEnumerator<StdPair<TKey, TValue>>
    {
        private Iterator current = ins.Begin();

        public StdPair<TKey, TValue> Current => current.Target;

        object IEnumerator.Current => Current;

        public readonly void Dispose() { }

        public bool MoveNext()
        {
            if (current != ins.End())
            {
                current++;
                return true;
            }
            return false;
        }

        public void Reset() => current = ins.Begin();
    }

    public static void DestructInstance(nint ptr)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<StdPair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    private bool _Lower_bound_duplicate(_Tree_node* _Bound, in TKey _Keyval)
    {
        return !_Bound->_IsNil && !StdLess<TKey>.Compare(_Keyval, _Bound->_Myval.First);
    }

    private ref struct _Tree_val()
    {
        public _Tree_node* _Myhead = null;
        public ulong _Mysize = 0;

        public static _Tree_node* _Max(_Tree_node* _Pnode)
        {
            while (!_Pnode->_Right->_IsNil)
            {
                _Pnode = _Pnode->_Right;
            }

            return _Pnode;
        }

        public static _Tree_node* _Min(_Tree_node* _Pnode)
        {
            while (!_Pnode->_Left->_IsNil)
            {
                _Pnode = _Pnode->_Left;
            }

            return _Pnode;
        }

        public void _Lrotate(_Tree_node* _Wherenode)
        {
            _Tree_node* _Pnode = _Wherenode->_Right;
            _Wherenode->_Right = _Pnode->_Left;

            if (!_Pnode->_Left->_IsNil)
                _Pnode->_Left->_Parent = _Wherenode;

            _Pnode->_Parent = _Wherenode->_Parent;

            if (_Wherenode == _Myhead->_Parent)
                _Myhead->_Parent = _Pnode;
            else if (_Wherenode == _Wherenode->_Parent->_Left)
                _Wherenode->_Parent->_Left = _Pnode;
            else
                _Wherenode->_Parent->_Right = _Pnode;

            _Pnode->_Left = _Wherenode;
            _Wherenode->_Parent = _Pnode;
        }

        public void _Rrotate(_Tree_node* _Wherenode)
        {
            _Tree_node* _Pnode = _Wherenode->_Left;
            _Wherenode->_Left = _Pnode->_Right;

            if (!_Pnode->_Right->_IsNil)
                _Pnode->_Right->_Parent = _Wherenode;

            _Pnode->_Parent = _Wherenode->_Parent;

            if (_Wherenode == _Myhead->_Parent)
                _Myhead->_Parent = _Pnode;
            else if (_Wherenode == _Wherenode->_Parent->_Right)
                _Wherenode->_Parent->_Right = _Pnode;
            else
                _Wherenode->_Parent->_Left = _Pnode;

            _Pnode->_Right = _Wherenode;
            _Wherenode->_Parent = _Pnode;
        }

        public _Tree_node* _Insert_node(_Tree_id _Loc, _Tree_node* _Newnode)
        {
            ++_Mysize;
            var _Head = _Myhead;
            _Newnode->_Parent = _Loc._Parent;
            if (_Loc._Parent == _Head)
            {
                _Head->_Left = _Newnode;
                _Head->_Parent = _Newnode;
                _Head->_Right = _Newnode;
                _Newnode->_Color = _Tree_node._Tree_color.Black;
                return _Newnode;
            }

            if (_Loc._Child == _Tree_child._Right)
            {
                _Loc._Parent->_Right = _Newnode;
                if (_Loc._Parent == _Head->_Parent)
                    _Head->_Right = _Newnode;
            }
            else
            {
                _Loc._Parent->_Left = _Newnode;
                if (_Loc._Parent == _Head->_Left)
                    _Head->_Left = _Newnode;
            }

            for (_Tree_node* _Pnode = _Newnode; _Pnode->_Parent->_Color == _Tree_node._Tree_color.Red;)
            {
                if (_Pnode->_Parent == _Pnode->_Parent->_Parent->_Left)
                {
                    var _Parent_sibling = _Pnode->_Parent->_Parent->_Right;
                    if (_Parent_sibling->_Color == _Tree_node._Tree_color.Red)
                    {
                        _Pnode->_Parent->_Color = _Tree_node._Tree_color.Black;
                        _Parent_sibling->_Color = _Tree_node._Tree_color.Black;
                        _Pnode->_Parent->_Parent->_Color = _Tree_node._Tree_color.Red;
                        _Pnode = _Pnode->_Parent->_Parent;
                    }
                    else
                    {
                        if (_Pnode == _Pnode->_Parent->_Right)
                        {
                            _Pnode = _Pnode->_Parent;
                            _Lrotate(_Pnode);
                        }

                        _Pnode->_Parent->_Color = _Tree_node._Tree_color.Black;
                        _Pnode->_Parent->_Parent->_Color = _Tree_node._Tree_color.Red;
                        _Rrotate(_Pnode->_Parent->_Parent);
                    }
                }
                else
                {
                    var _Parent_sibling = _Pnode->_Parent->_Parent->_Left;
                    if (_Parent_sibling->_Color == _Tree_node._Tree_color.Red)
                    {
                        _Pnode->_Parent->_Color = _Tree_node._Tree_color.Black;
                        _Parent_sibling->_Color = _Tree_node._Tree_color.Black;
                        _Pnode->_Parent->_Parent->_Color = _Tree_node._Tree_color.Red;
                        _Pnode = _Pnode->_Parent->_Parent;
                    }
                    else
                    {
                        if (_Pnode == _Pnode->_Parent->_Left)
                        {
                            _Pnode = _Pnode->_Parent;
                            _Rrotate(_Pnode);
                        }

                        _Pnode->_Parent->_Color = _Tree_node._Tree_color.Black;
                        _Pnode->_Parent->_Parent->_Color = _Tree_node._Tree_color.Red;
                        _Lrotate(_Pnode->_Parent->_Parent);
                    }
                }
            }

            _Head->_Parent->_Color = _Tree_node._Tree_color.Black;
            return _Newnode;
        }
    }

    private enum _Tree_child
    {
        _Right,
        _Left,
        _Unused
    };

    private ref struct _Tree_id
    {
        public _Tree_node* _Parent;
        public _Tree_child _Child;
    }

    private ref struct _Tree_find_result
    {
        public _Tree_id _Location;
        public _Tree_node* _Bound;
    }

    private _Tree_find_result _Find_upper_bound(in TKey key)
    {
        var _Scary = _Get_scary();
        _Tree_find_result _Result = new()
        {
            _Location = new()
            {
                _Parent = _Scary->_Myhead->_Parent,
                _Child = _Tree_child._Right
            },
            _Bound = _Scary->_Myhead
        };
        _Tree_node* _Trynode = _Result._Location._Parent;
        while (!_Trynode->_IsNil)
        {
            if (StdLess<TKey>.Compare(key, _Trynode->_Myval.First))
            {
                _Result._Location._Child = _Tree_child._Left;
                _Result._Bound = _Trynode;
                _Trynode = _Trynode->_Left;
            }
            else
            {
                _Result._Location._Child = _Tree_child._Right;
                _Trynode = _Trynode->_Right;
            }
        }

        return _Result;
    }

    private _Tree_find_result _Find_lower_bound(in TKey key)
    {
        var _Scary = _Get_scary();
        _Tree_find_result _Result = new()
        {
            _Location = new()
            {
                _Parent = _Scary->_Myhead->_Parent,
                _Child = _Tree_child._Right
            },
            _Bound = _Scary->_Myhead
        };
        _Tree_node* _Trynode = _Result._Location._Parent;

        while (!_Trynode->_IsNil)
        {
            _Result._Location._Parent = _Trynode;
            if (StdLess<TKey>.Compare(_Trynode->_Myval.First, key))
            {
                _Result._Location._Child = _Tree_child._Right;
                _Trynode = _Trynode->_Right;
            }
            else
            {
                _Result._Location._Child = _Tree_child._Left;
                _Result._Bound = _Trynode;
                _Trynode = _Trynode->_Left;
            }
        }

        return _Result;

    }

    private _Tree_val* _Get_scary() => _This;

    private StdPair<ValuePointer<_Tree_node>, bool> _Emplace(ref StdPair<TKey, TValue> value)
    {
        var _Scary = _Get_scary();
        _Tree_find_result _Loc;
        _Tree_node* _Inserted = NativeAlloc<_Tree_node>.New();
        Unsafe.Copy(&_Inserted->_Myval, ref value);
        _Inserted->_Left = _Scary->_Myhead;
        _Inserted->_Parent = _Scary->_Myhead;
        _Inserted->_Right = _Scary->_Myhead;
        _Inserted->_Color = _Tree_node._Tree_color.Red;
        _Inserted->_IsNil = false;

        ref TKey _Keyval = ref _Inserted->_Myval.First;
        _Loc = _Find_lower_bound(_Keyval);
        if (_Lower_bound_duplicate(_Loc._Bound, _Keyval))
        {
            return new(_Loc._Bound, false);
        }

        return new(_Scary->_Insert_node(_Loc._Location, _Inserted), true);
    }

    private StdPair<ValuePointer<_Tree_node>, bool> _Try_emplace(in TKey key, in TValue value)
    {
        var _Loc = _Find_lower_bound(key);
        if (_Lower_bound_duplicate(_Loc._Bound, key))
            return new(_Loc._Bound, false);

        var _Scary = _Get_scary();
        _Tree_node* _Inserted = NativeAlloc<_Tree_node>.New();
        var _Keyval = new StdPair<TKey, TValue>(key, value);
        Unsafe.Copy(&_Inserted->_Myval, ref _Keyval);
        _Inserted->_Left = _Scary->_Myhead;
        _Inserted->_Parent = _Scary->_Myhead;
        _Inserted->_Right = _Scary->_Myhead;
        _Inserted->_Color = _Tree_node._Tree_color.Red;
        _Inserted->_IsNil = false;

#pragma warning disable CS9080
        return new(_Scary->_Insert_node(_Loc._Location, _Inserted), true);
#pragma warning restore CS9080
    }

    public Iterator Begin() => new(_This->_Myhead->_Left);

    public Iterator End() => new(_This->_Myhead);

    public ReverseIterator<Iterator, StdPair<TKey, TValue>> RBegin() => new(new(_This->_Myhead));

    public ReverseIterator<Iterator, StdPair<TKey, TValue>> REnd() => new(new(_This->_Myhead->_Left));

    public ulong Size() => _Get_scary()->_Mysize;

    public ref TValue At(in TKey key)
    {

        var _Loc = _Find_lower_bound(key);
        if (!_Lower_bound_duplicate(_Loc._Bound, key))
            throw new KeyNotFoundException($"invalid map<{typeof(TKey).Name}, {typeof(TValue).Name}> key");

        return ref _Loc._Bound->_Myval.Second;
    }

    public ref TValue this[in TKey key] => ref _Try_emplace(key, default).First.Target._Myval.Second;

    public StdPair<Iterator, bool> Emplace(in StdPair<TKey, TValue> value)
    {
        fixed (StdPair<TKey, TValue>* valuePtr = &value)
        {
            var _Loc = _Emplace(ref *valuePtr);
            return new(new(_Loc.First), _Loc.Second);
        }
    }
}
