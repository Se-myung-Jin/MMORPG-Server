using Google.Protobuf.Protocol;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Player : GameObject
    {
        public int PlayerDbId { get; set; }
        public ClientSession Session { get; set; }
        public VisionCube Vision { get; private set; }
        public Inventory Inven { get; private set; } = new Inventory();

        public int WeaponDamage { get; private set; }
        public int ArmorDefence { get; private set; }

        public override int TotalAttack { get { return Stat.Attack + WeaponDamage; } }
        public override int TotalDefence { get { return ArmorDefence; } }

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Vision = new VisionCube(this);
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public void OnLeaveGame()
        {
            // TODO
            // DB 연동?
            // -- 피가 깎일 때마다 DB 접근할 필요가 있을까?
            // 1) 서버 다운되면 아직 저장되지 않은 정보 날아감
            // 2) 코드 흐름을 다 막아버린다 !!!!
            // - 비동기(Async) 방법 사용?
            // - 다른 쓰레드로 DB 일감을 던져버리면 되지 않을까?
            // -- 결과를 받아서 이어서 처리를 해야 하는 경우가 많음.
            // -- 아이템 생성

            // 서빙 담당
            // 결제 담당
            /*using (AppDbContext db = new AppDbContext())
            {
                PlayerDb playerDb = new PlayerDb();
                playerDb.PlayerDbId = PlayerDbId;
                playerDb.Hp = Stat.Hp;

                db.Entry(playerDb).State = EntityState.Unchanged;
                db.Entry(playerDb).Property(nameof(PlayerDb.Hp)).IsModified = true;
                db.SaveChangesEx();

                Console.WriteLine($"Hp Saved({playerDb.Hp})");
            }*/

            DbTransaction.SavePlayerStatus_AllInOne(this, Room);
        }

        public void HandleEquipItem(C_EquipItem equipPacket)
        {
            Item item = Inven.Get(equipPacket.ItemDbId);
            if (item == null)
                return;

            if (item.ItemType == ItemType.Consumable)
                return;

            // 착용 요청이라면 겹치는 부위 해제
            if (equipPacket.Equipped)
            {
                Item unequipItem = null;

                if (item.ItemType == ItemType.Weapon)
                {
                    unequipItem = Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);
                }
                else if (item.ItemType == ItemType.Armor)
                {
                    ArmorType armorType = ((Armor)item).ArmorType;
                    unequipItem = Inven.Find(i => i.Equipped && i.ItemType == ItemType.Armor && ((Armor)i).ArmorType == armorType);
                }

                if (unequipItem != null)
                {
                    // 메모리 선적용
                    unequipItem.Equipped = false;

                    // DB에 Noti
                    DbTransaction.EquipItemNoti(this, unequipItem);

                    // 클라에 통보 TODO
                    S_EquipItem equipOkItem = new S_EquipItem();
                    equipOkItem.ItemDbId = unequipItem.ItemDbId;
                    equipOkItem.Equipped = false;
                    Session.Send(equipOkItem);
                }
            }

            {
                // 메모리 선적용
                item.Equipped = equipPacket.Equipped;

                // DB에 Noti
                DbTransaction.EquipItemNoti(this, item);

                // 클라에 통보 TODO
                S_EquipItem equipOkItem = new S_EquipItem();
                equipOkItem.ItemDbId = equipPacket.ItemDbId;
                equipOkItem.Equipped = equipPacket.Equipped;
                Session.Send(equipOkItem);
            }

            RefreshAdditionalStat();
        }

        public void RefreshAdditionalStat()
        {
            WeaponDamage = 0;
            ArmorDefence = 0;

            foreach (Item item in Inven.Items.Values)
            {
                if (item.Equipped == false)
                    continue;

                switch (item.ItemType)
                {
                    case ItemType.Weapon:
                        WeaponDamage += ((Weapon)item).Damage;
                        break;
                    case ItemType.Armor:
                        ArmorDefence += ((Armor)item).Defence;
                        break;
                }
            }
        }
    }
}
