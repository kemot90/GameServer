using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class Skill
    {
        private uint id;
        private uint accessLevel;
        private uint strength;
        private uint stamina;
        private uint dexterity;
        private uint luck;

        public Skill(uint _id, uint _accessLevel, uint _strength, uint _stamina, uint _dexterity, uint _luck)
        {
            id = _id;
            accessLevel = _accessLevel;
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

        public uint AccessLevel
        {
            get
            {
                return accessLevel;
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

    class Skills
    {
        private List<Skill> skillList;

        public Skills(GlobalMySql dataBase)
        {
            skillList = new List<Skill>();

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
            query.CommandText = "SELECT * FROM `skills`";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Skill skill = new Skill(
                            reader.GetUInt32("id_skill"),
                            reader.GetUInt32("dostep_lv"),
                            reader.GetUInt32("strength"),
                            reader.GetUInt32("stamina"),
                            reader.GetUInt32("dexterity"),
                            reader.GetUInt32("luck")
                            );
                        skillList.Add(skill);
                    }
                }
            }
            catch
            {
                //
            }
        }

        public List<Skill> SkillList
        {
            get
            {
                return skillList;
            }
        }
    }
}
