using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public partial class DbTransaction : JobSerializer
    {
        public static void EquipItemNoti(Player player, Item item)
        {
            if (player == null || item == null)
                return;

            ItemDb itemDb = new ItemDb()
            {
                ItemDbId = item.ItemDbId,
                Equipped = item.Equipped,
            };

            // You
            Instance.Push(() =>
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Entry(itemDb).State = EntityState.Unchanged;
                    db.Entry(itemDb).Property(nameof(ItemDb.Equipped)).IsModified = true;

                    bool success = db.SaveChangesEx();
                    if (!success)
                    {
                        // 실패했으면 Kick
                    }
                }
            });
        }
    }
}
