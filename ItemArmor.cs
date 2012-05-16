using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class ItemArmor
    {
        private uint id;
        private string type;
        private uint price;
        private string name;
        private string part;
        private uint armor;
        private uint strength;
        private uint stamina;
        private uint dexterity;
        private uint luck;

        public ItemArmor(uint _id, string _type, uint _price, string _name, string _part, uint _armor, uint _strength, uint _stamina, uint _dexterity, uint _luck)
        {
            id = _id;
            type = _type;
            price = _price;
            name = _name;
            part = _part;
            armor = _armor;
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

        public string Part
        {
            get
            {
                return part;
            }
        }

        public uint Armor
        {
            get
            {
                return armor;
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

    class ItemsArmor
    {
        private List<ItemArmor> itemAList;

        public ItemsArmor(GlobalMySql dataBase)
        {
            itemAList = new List<ItemArmor>();

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
            query.CommandText = "SELECT ITEM.*, ARMOR_DETAILS.part, ARMOR_DETAILS.armor, BONUSES.strength, BONUSES.stamina, BONUSES.dexterity, BONUSES.luck FROM `item` ITEM, `armor_details` ARMOR_DETAILS, `bonuses` BONUSES WHERE ITEM.type='armor' AND ITEM.id=ARMOR_DETAILS.id AND ITEM.id=BONUSES.id";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ItemArmor armor = new ItemArmor(
                            reader.GetUInt32("id"),
                            reader.GetString("type"),
                            reader.GetUInt32("price"),
                            reader.GetString("name"),
                            reader.GetString("part"),
                            reader.GetUInt32("armor"),
                            reader.GetUInt32("strength"),
                            reader.GetUInt32("stamina"),
                            reader.GetUInt32("dexterity"),
                            reader.GetUInt32("luck")
                            );
                        itemAList.Add(armor);
                    }
                }
            }
            catch
            {
                //
            }

        }

        public List<ItemArmor> ItemAList
        {
            get
            {
                return itemAList;
            }
        }


    }


}
