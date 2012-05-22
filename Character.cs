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
        private ulong lastDamage;
        private ulong damage;
        private ulong lastFatigue;
        private ulong fatigue;
        private ulong travelEndTime;
        private uint travelDestination;
        private string status;

        //obiekt bieżącego wyposażenia
        private CharacterEquipment equipment;

        //obiekt przedmiotów gracza
        private CharacterStorage storage;

        private GlobalMySql dataBase;

        public Character(ulong playerId, GlobalMySql GlobalMySqlObject)
        {
            //kosntruktor postaci

            //przypisanie identyfikatora gracza do indetyfikatora postaci
            this.Id = playerId;

            //przypisanie obiektu zawierającego ustawienia i połączenie z bazą do obiektu postaci 
            dataBase = GlobalMySqlObject;

            equipment = new CharacterEquipment(playerId, GlobalMySqlObject);
            storage = new CharacterStorage(playerId, dataBase);

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

            //zapytanie pobierające podstawowe dane postaci
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

            query.CommandText = "SELECT * FROM `character_status` WHERE `character_status`.`id` = " + Id;

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        status = reader.GetString("status");
                        lastDamage = reader.GetUInt64("lastDamage");
                        damage = reader.GetUInt64("damage");
                        lastFatigue = reader.GetUInt64("lastFatigue");
                        fatigue = reader.GetUInt64("fatigue");

                        location = reader.GetUInt32("location");
                        travelEndTime = reader.GetUInt64("travelEndTime");
                        travelDestination = reader.GetUInt32("travelDestination");
                    }
                }
            }
            catch
            {
                //
            }
        }

        #region Akcesory

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

        public CharacterStorage CharacterSotrage
        {
            get
            {
                return storage;
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

        public ulong LastDamage
        {
            get
            {
                return lastDamage;
            }
            set
            {
                lastDamage = value;
            }
        }

        public ulong Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        public ulong LastFatigue
        {
            get
            {
                return lastFatigue;
            }
        }

        public ulong Fatigue
        {
            get
            {
                return fatigue;
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

        public ulong TravelEndTime
        {
            get
            {
                return travelEndTime;
            }
            set
            {
                travelEndTime = value;
            }
        }

        public uint TravelDestination
        {
            get
            {
                return travelDestination;
            }
            set
            {
                travelDestination = value;
            }
        }

        public ulong getHP()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
