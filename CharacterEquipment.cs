using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public class CharacterEquipment
    {
        public enum Part { Head, Shoulders, Chest, Hands, Thighs, Legs, Weapon, Shield };
        private ulong characterId;
        private uint head;
        private uint shoulders;
        private uint chest;
        private uint hands;
        private uint thighs;
        private uint legs;
        private uint weapon;
        private uint shield;

        public uint Head
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Shoulders
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Chest
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Hands
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Thighs
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Legs
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Weapon
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public uint Shield
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public void wear(Part bodyPart, int idItem)
        {
            throw new System.NotImplementedException();
        }

        public void takeoff(Part bodyPart)
        {
            throw new System.NotImplementedException();
        }
    }
}
