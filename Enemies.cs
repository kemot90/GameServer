using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Commands;
using System.Data;
using MySql.Data.MySqlClient;

namespace GameServer
{
    class Enemies
    {
        //tutaj będą trzymane wszystkie potworki w danej okolicy
        private List<Mob> enemiesList;
        private uint mobsCount = 0;

        private GlobalMySql dataBase;

        //konstruktor
        public Enemies(uint location, GlobalMySql GlobalMySqlObject)
        {
            enemiesList = new List<Mob>();

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

            //wybierze potwory przypisane do zadanej lokalizacji
            query.CommandText = "SELECT C.id_creature, C.name, C.level, C.bonusHP, C.strength, C.luck, C.dexterity, C.stamina, C.gold_drop, C.exp, C.icon_name FROM creature C, wystepowanie W WHERE (W.id_surr = " + location + " AND W.id_creature = C.id_creature)";

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Mob mb = new Mob(
                            reader.GetUInt64("id_creature"),
                            reader.GetString("name"),
                            reader.GetUInt32("level"),
                            reader.GetUInt64("bonusHP"),
                            reader.GetUInt32("strength"),
                            reader.GetUInt32("luck"),
                            reader.GetUInt32("dexterity"),
                            reader.GetUInt32("stamina"),
                            reader.GetUInt32("gold_drop"),
                            reader.GetUInt32("exp"),
                            reader.GetString("icon_name")
                            );

                        enemiesList.Add(mb);
                        ++mobsCount;
                    }
                }
            }
            catch
            {
                //
            }
        }

        public List<Mob> EnemiesList
        {
            get
            {
                return enemiesList;
            }
        }

        public uint MobsCount
        {
            get
            {
                return mobsCount;
            }
        }
    }
}

