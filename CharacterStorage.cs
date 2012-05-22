using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Commands;

namespace GameServer
{
    public class Item
    {
        private uint id;
        private string name;
        private string type;
        private uint price;
        private string icon;

        public Item(uint id, string name, string type, uint price, string icon)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.price = price;
            this.icon = icon;
        }

        #region Akcesory
        public uint Id
        {
            get
            {
                return id;
            }
        }
        public string Name
        {
            get
            {
                return name;
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
        public string Icon
        {
            get
            {
                return icon;
            }
        }
        #endregion
    }

    public class Storage
    {
        private ulong id; //id gracza
        private uint itemid;
        private uint amount;

        public Storage(ulong id, uint itemid, uint amount)
        {
            this.id = id;
            this.itemid = itemid;
            this.amount = amount;
        }

        #region Akcesory
        public ulong CharacterId
        {
            get
            {
                return id;
            }
        }

        public uint ItemId
        {
            get
            {
                return itemid;
            }
        }

        public uint Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
            }
        }
        #endregion
    }

    public class CharacterStorage
    {
        private GlobalMySql dataBase;
        private ulong id;
        //private List<Item> items;
        private List<Storage> storage;

        public CharacterStorage(ulong id, GlobalMySql GlobalMySqlObject)
        {
            //items = new List<Item>();
            storage = new List<Storage>();

            dataBase = GlobalMySqlObject;
            this.id = id;

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Character_storage: Nie udało się połączyć z bazą danych");
                    return;
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();

            query.CommandText = "SELECT S.* FROM `character_storage` S WHERE S.id = " + this.id;

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Storage position = new Storage(
                            reader.GetUInt32("id"),
                            reader.GetUInt32("id_item"),
                            reader.GetUInt32("amount")
                            );
                        storage.Add(position);
                    }
                }
            }
            catch
            {
                //
            }
        }

        public void AddItem(uint itemId, uint amount)
        {
            //pobierz pozycję z magazynu postaci
            Storage position = storage.Find(pos => pos.ItemId == itemId);

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Character_storage: Nie udało się połączyć z bazą danych");
                    return;
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();

            if (position == null)
            {
                storage.Add(new Storage(this.id, itemId, amount));
                query.CommandText = "INSERT INTO `gierka`.`character_storage` (`id`, `id_item`, `amount`) VALUES ('" + this.id + "', '" + itemId + "', '" + amount + "')";
                query.ExecuteNonQuery();
            }
            else
            {
                position.Amount += amount;
                query.CommandText = "UPDATE `gierka`.`character_storage` SET `amount` = '" + position.Amount + "' WHERE `character_storage`.`id` =" + this.id + " AND `character_storage`.`id_item` =" + itemId;
                query.ExecuteNonQuery();
            }
        }

        public void RemoveOneItem(uint itemId)
        {
            //pobierz pozycję z magazynu postaci
            Storage position = storage.Find(pos => pos.ItemId == itemId);

            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Character_storage: Nie udało się połączyć z bazą danych");
                    return;
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();

            //jeżeli ma więcej niż tylko jeden egzemplarz to tylko zmniejsz jego liczbę
            if (position.Amount > 1)
            {
                position.Amount--;

                query.CommandText = "UPDATE `gierka`.`character_storage` SET `amount` = '" + position.Amount + "' WHERE `character_storage`.`id` =" + this.id + " AND `character_storage`.`id_item` =" + itemId;

                query.ExecuteNonQuery();
            }
            else
            {
                storage.RemoveAll(pos => pos.ItemId == itemId);

                query.CommandText = "DELETE FROM `gierka`.`character_storage` WHERE `character_storage`.`id` = " + this.id + " AND `character_storage`.`id_item` = " + itemId;

                query.ExecuteNonQuery();
            }
        }

        public Item GetItemById(uint id)
        {
            if (dataBase.Connection.State != ConnectionState.Open)
            {
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Character_storage: Nie udało się połączyć z bazą danych");
                    return null;
                }
            }

            MySqlCommand query = dataBase.Connection.CreateCommand();

            query.CommandText = "SELECT * FROM `item` WHERE `id` = " + id;

            try
            {
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Item item = new Item(
                            reader.GetUInt32("id"),
                            reader.GetString("name"),
                            reader.GetString("type"),
                            reader.GetUInt32("price"),
                            reader.GetString("icon")
                            );
                        return item;
                    }
                }
            }
            catch
            {
                //
            }
            return null;
        }

        public List<Storage> StorageList
        {
            get
            {
                return storage;
            }
        }
    }
}
