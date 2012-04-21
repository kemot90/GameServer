using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace GameServer
{
    public class CharacterEquipment
    {
        public enum Part { Head, Chest, Legs, Weapon, Shield };
        private ulong id;
        private uint head;
        private uint chest;
        private uint legs;
        private uint weapon;
        private uint shield;

        private GlobalMySql dataBase;

        public CharacterEquipment(ulong characterId, GlobalMySql GlobalMySqlObject)
        {
            //przypisanie identyfikatora postaci
            id = characterId;

            //przypisanie obiektu zawierającego ustawienia i połączenie z bazą do obiektu postaci
            dataBase = GlobalMySqlObject;

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    //
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();
            query.CommandText = "SELECT * FROM `character_equipment` WHERE `character_equipment`.`id` = " + id;

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        head = reader.GetUInt32("head");
                        chest = reader.GetUInt32("chest");
                        legs = reader.GetUInt32("legs");
                        weapon = reader.GetUInt32("weapon");
                        shield = reader.GetUInt32("shield");
                    }
                }
            }
            catch
            {
                //
            }
        }

        public uint Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }

        public uint Chest
        {
            get
            {
                return chest;
            }
            set
            {
                chest = value;
            }
        }

        public uint Legs
        {
            get
            {
                return legs;
            }
            set
            {
                legs = value;
            }
        }

        public uint Weapon
        {
            get
            {
                return weapon;
            }
            set
            {
                weapon = value;
            }
        }

        public uint Shield
        {
            get
            {
                return shield;
            }
            set
            {
                shield = value;
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
