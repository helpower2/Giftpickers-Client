﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    Welcome = 1,
    SpawnPlayer = 2,
    PlayerPosition = 3,
    PlayerRotation = 4,
    ObjectTransform = 5,
    SpawnPrefab = 6,
    ChatMassage = 7
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    WelcomeReceived = 1,
    PlayerMovement = 2,
    ChatMassage = 3
}

public class Packet : IDisposable
{
    private List<byte> _buffer;

    private bool _disposed;
    private byte[] _readableBuffer;
    private int _readPos;

    /// <summary>Creates a new empty packet (without an ID).</summary>
    public Packet()
    {
        _buffer = new List<byte>(); // Initialize buffer
        _readPos = 0; // Set readPos to 0
    }

    /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
    /// <param name="_id">The packet ID.</param>
    public Packet(int _id)
    {
        _buffer = new List<byte>(); // Initialize buffer
        _readPos = 0; // Set readPos to 0

        Write(_id); // Write packet id to the buffer
    }

    /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public Packet(byte[] _data)
    {
        _buffer = new List<byte>(); // Initialize buffer
        _readPos = 0; // Set readPos to 0

        SetBytes(_data);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _disposing)
    {
        if (!_disposed)
        {
            if (_disposing)
            {
                _buffer = null;
                _readableBuffer = null;
                _readPos = 0;
            }

            _disposed = true;
        }
    }

    #region Functions

    /// <summary>Sets the packet's content and prepares it to be read.</summary>
    /// <param name="_data">The bytes to add to the packet.</param>
    public void SetBytes(byte[] _data)
    {
        Write(_data);
        _readableBuffer = _buffer.ToArray();
    }

    /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
    public void WriteLength()
    {
        _buffer.InsertRange(0,
            BitConverter.GetBytes(_buffer.Count)); // Insert the byte length of the packet at the very beginning
    }

    /// <summary>Inserts the given int at the start of the buffer.</summary>
    /// <param name="_value">The int to insert.</param>
    public void InsertInt(int _value)
    {
        _buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insert the int at the start of the buffer
    }

    /// <summary>Gets the packet's content in array form.</summary>
    public byte[] ToArray()
    {
        _readableBuffer = _buffer.ToArray();
        return _readableBuffer;
    }

    /// <summary>Gets the length of the packet's content.</summary>
    public int Length()
    {
        return _buffer.Count; // Return the length of buffer
    }

    /// <summary>Gets the length of the unread data contained in the packet.</summary>
    public int UnreadLength()
    {
        return Length() - _readPos; // Return the remaining length (unread)
    }

    /// <summary>Resets the packet instance to allow it to be reused.</summary>
    /// <param name="_shouldReset">Whether or not to reset the packet.</param>
    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            _buffer.Clear(); // Clear buffer
            _readableBuffer = null;
            _readPos = 0; // Reset readPos
        }
        else
        {
            _readPos -= 4; // "Unread" the last read int
        }
    }

    #endregion

    #region Write Data

    /// <summary>Adds a byte to the packet.</summary>
    /// <param name="_value">The byte to add.</param>
    public void Write(byte _value)
    {
        _buffer.Add(_value);
    }

    /// <summary>Adds an array of bytes to the packet.</summary>
    /// <param name="_value">The byte array to add.</param>
    public void Write(byte[] _value)
    {
        _buffer.AddRange(_value);
    }

    /// <summary>Adds a short to the packet.</summary>
    /// <param name="_value">The short to add.</param>
    public void Write(short _value)
    {
        _buffer.AddRange(BitConverter.GetBytes(_value));
    }

    /// <summary>Adds an int to the packet.</summary>
    /// <param name="_value">The int to add.</param>
    public void Write(int _value)
    {
        _buffer.AddRange(BitConverter.GetBytes(_value));
    }

    /// <summary>Adds a long to the packet.</summary>
    /// <param name="_value">The long to add.</param>
    public void Write(long _value)
    {
        _buffer.AddRange(BitConverter.GetBytes(_value));
    }

    /// <summary>Adds a float to the packet.</summary>
    /// <param name="_value">The float to add.</param>
    public void Write(float _value)
    {
        _buffer.AddRange(BitConverter.GetBytes(_value));
    }

    /// <summary>Adds a bool to the packet.</summary>
    /// <param name="_value">The bool to add.</param>
    public void Write(bool _value)
    {
        _buffer.AddRange(BitConverter.GetBytes(_value));
    }

    /// <summary>Adds a string to the packet.</summary>
    /// <param name="_value">The string to add.</param>
    public void Write(string _value)
    {
        Write(_value.Length); // Add the length of the string to the packet
        _buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Add the string itself
    }

    /// <summary>Adds a Vector3 to the packet.</summary>
    /// <param name="_value">The Vector3 to add.</param>
    public void Write(Vector3 _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
    }

    /// <summary>Adds a Quaternion to the packet.</summary>
    /// <param name="_value">The Quaternion to add.</param>
    public void Write(Quaternion _value)
    {
        Write(_value.x);
        Write(_value.y);
        Write(_value.z);
        Write(_value.w);
    }

    #endregion

    #region Read Data

    /// <summary>Reads a byte from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte ReadByte(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = _readableBuffer[_readPos]; // Get the byte at readPos' position
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += 1; // Increase readPos by 1
            return _value; // Return the byte
        }

        throw new Exception("Could not read value of type 'byte'!");
    }

    /// <summary>Reads an array of bytes from the packet.</summary>
    /// <param name="_length">The length of the byte array.</param>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value =
                _buffer.GetRange(_readPos, _length)
                    .ToArray(); // Get the bytes at readPos' position with a range of _length
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += _length; // Increase readPos by _length
            return _value; // Return the bytes
        }

        throw new Exception("Could not read value of type 'byte[]'!");
    }

    /// <summary>Reads a short from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public short ReadShort(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = BitConverter.ToInt16(_readableBuffer, _readPos); // Convert the bytes to a short
            if (_moveReadPos)
                // If _moveReadPos is true and there are unread bytes
                _readPos += 2; // Increase readPos by 2
            return _value; // Return the short
        }

        throw new Exception("Could not read value of type 'short'!");
    }

    /// <summary>Reads an int from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public int ReadInt(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = BitConverter.ToInt32(_readableBuffer, _readPos); // Convert the bytes to an int
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += 4; // Increase readPos by 4
            return _value; // Return the int
        }

        throw new Exception("Could not read value of type 'int'!");
    }

    /// <summary>Reads a long from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public long ReadLong(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = BitConverter.ToInt64(_readableBuffer, _readPos); // Convert the bytes to a long
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += 8; // Increase readPos by 8
            return _value; // Return the long
        }

        throw new Exception("Could not read value of type 'long'!");
    }

    /// <summary>Reads a float from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = BitConverter.ToSingle(_readableBuffer, _readPos); // Convert the bytes to a float
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += 4; // Increase readPos by 4
            return _value; // Return the float
        }

        throw new Exception("Could not read value of type 'float'!");
    }

    /// <summary>Reads a bool from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (_buffer.Count > _readPos)
        {
            // If there are unread bytes
            var _value = BitConverter.ToBoolean(_readableBuffer, _readPos); // Convert the bytes to a bool
            if (_moveReadPos)
                // If _moveReadPos is true
                _readPos += 1; // Increase readPos by 1
            return _value; // Return the bool
        }

        throw new Exception("Could not read value of type 'bool'!");
    }

    /// <summary>Reads a string from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            var _length = ReadInt(); // Get the length of the string
            var _value = Encoding.ASCII.GetString(_readableBuffer, _readPos, _length); // Convert the bytes to a string
            if (_moveReadPos && _value.Length > 0)
                // If _moveReadPos is true string is not empty
                _readPos += _length; // Increase readPos by the length of the string
            return _value; // Return the string
        }
        catch
        {
            throw new Exception("Could not read value of type 'string'!");
        }
    }

    /// <summary>Reads a Vector3 from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Vector3 ReadVector3(bool _moveReadPos = true)
    {
        return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
    }

    /// <summary>Reads a Quaternion from the packet.</summary>
    /// <param name="_moveReadPos">Whether or not to move the buffer's read position.</param>
    public Quaternion ReadQuaternion(bool _moveReadPos = true)
    {
        return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos),
            ReadFloat(_moveReadPos));
    }

    #endregion
}