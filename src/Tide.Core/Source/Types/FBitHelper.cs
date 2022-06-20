using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class FBitHelper
    {
        public static int SetBit(int bit, bool val, int _int)
        {
            if (val)
            {
                _int |= 1 << bit;
            }
            else
            {
                _int &= ~(1 << bit);
            }
            return _int;
        }

        public static void SetBit(int bit, bool val, ref int _int)
        {
            if (val)
            {
                _int |= 1 << bit;
            }
            else
            {
                _int &= ~(1 << bit);
            }
        }

        public static uint SetBit(int bit, bool val, uint _int)
        {
            if (val)
            {
                _int |= 1u << bit;
            }
            else
            {
                _int &= ~(1u << bit);
            }
            return _int;
        }

        public static void SetBit(int bit, bool val, ref uint _int)
        {
            if (val)
            {
                _int |= 1u << bit;
            }
            else
            {
                _int &= ~(1u << bit);
            }
        }
    }
}
