using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class ItemWeapon
    {
        private uint id;
        private string type;
        private uint price;
        private string name;
        private uint min_attack;
        private uint max_attack;
        private uint strength;
        private uint stamina;
        private uint dexterity;
        private uint luck;

        public ItemWeapon(uint _id, string _type, uint _price, string _name, uint _min_attack, uint _max_attack, uint _strength, uint _stamina, uint _dexterity, uint _luck)
        {
            id = _id;
            type = _type;
            price = _price;
            name = _name;
            min_attack = _min_attack;
            max_attack = _max_attack;
            strength = _strength;
            stamina = _stamina;
            dexterity = _dexterity;
            luck = _luck;
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public uint Price
        {
            get
            {
                return price;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public uint Min_attack
        {
            get
            {
                return min_attack;
            }
        }

        public uint Max_attack
        {
            get
            {
                return max_attack;
            }
        }

        public uint Strength
        {
            get
            {
                return strength;
            }
        }

        public uint Stamina
        {
            get
            {
                return stamina;
            }
        }

        public uint Dexterity
        {
            get
            {
                return dexterity;
            }
        }

        public uint Luck
        {
            get
            {
                return luck;
            }
        }

    }

    class ItemsWeapon
    {
        private List<ItemWeapon> itemWList;

        public ItemsWeapon(GlobalMySql dataBase)
        {
            itemWList = new List<ItemWeapon>();

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
            query.CommandText = "SELECT ITEM.*, WEAPON_DETAILS.min_attack, WEAPON_DETAILS.max_attack, BONUSES.strength, BONUSES.stamina, BONUSES.dexterity, BONUSES.luck FROM `item` ITEM, `weapon_details` WEAPON_DETAILS, `bonuses` BONUSES WHERE ITEM.type='weapon' AND ITEM.id=WEAPON_DETAILS.id AND ITEM.id=BONUSES.id";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemWeapon weapon = new ItemWeapon(
                            reader.GetUInt32("id"),
                            reader.GetString("type"),
                            reader.GetUInt32("price"),
                            reader.GetString("name"),
                            reader.GetUInt32("min_attack"),
                            reader.GetUInt32("max_attack"),
                            reader.GetUInt32("strength"),
                            reader.GetUInt32("stamina"),
                            reader.GetUInt32("dexterity"),
                            reader.GetUInt32("luck")
                            );
                        itemWList.Add(weapon);
                    }
                }
            }
            catch
            {
                //
            }

        }

        public List<ItemWeapon> ItemWList
        {
            get
            {
                return itemWList;
            }
        }


    }
}



    

