using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace GameServer
{
    public class CharacterStatus
    {
        public const string IN_STANDBY = "IN_STANDBY";
        public const string IS_TRAVELING = "IS_TRAVELING";
        public const string IS_DEAD = "IS_DEAD";
    }
    public class Character : Creature
    {
        private ulong exp;
        private ulong gold;
        private ulong lastHPChange;
        private ulong damage;
        private ulong travelEndTime;
        private string status;

        //obiekt bieżącego wyposażenia
        private CharacterEquipment equipment;

        private GlobalMySql dataBase;

        public Character(ulong playerId, GlobalMySql GlobalMySqlObject)
        {
            //kosntruktor postaci

            //przypisanie identyfikatora gracza do indetyfikatora postaci
            this.Id = playerId;

            //przypisanie obiektu zawierającego ustawienia i połączenie z bazą do obiektu postaci 
            dataBase = GlobalMySqlObject;

            equipment = new CharacterEquipment(playerId, GlobalMySqlObject);

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
            query.CommandText = "SELECT * FROM `character` WHERE `character`.`id` = " + Id;

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = reader.GetString("name");
                        level = reader.GetUInt32("level");
                        strength = reader.GetUInt32("strength");
                        stamina = reader.GetUInt32("stamina");
                        dexterity = reader.GetUInt32("dexterity");
                        luck = reader.GetUInt32("luck");

                        exp = reader.GetUInt32("exp");
                        gold = reader.GetUInt32("gold");
                    }
                }
            }
            catch
            {
                //
            }
        }

        public ulong Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;

            }
        }

        public uint Dexterity
        {
            get
            {
                return dexterity;
            }
            set
            {
                dexterity = value;
            }
        }

        public uint Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }

        public uint Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public uint Luck
        {
            get
            {
                return luck;
            }
            set
            {
                luck = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public uint Stamina
        {
            get
            {
                return stamina;
            }
            set
            {
                stamina = value;
            }
        }

        public uint Strength
        {
            get
            {
                return strength;
            }
            set
            {
                strength = value;
            }
        }

        public ulong Experience
        {
            get
            {
                return exp;
            }
            set
            {
                exp = value;
            }
        }

        public ulong Gold
        {
            get
            {
                return gold;
            }
            set
            {
                gold = value;
            }
        }

        public int Head
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Shoulders
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Chest
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Hands
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Thighs
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Legs
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Weapon
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Shield
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public CharacterEquipment Equipment
        {
            get
            {
                return equipment;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public ulong getHP()
        {
            throw new System.NotImplementedException();
        }
    }
}
