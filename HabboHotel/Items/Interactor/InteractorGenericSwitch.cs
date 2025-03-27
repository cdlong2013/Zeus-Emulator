using MySqlX.XDevAPI;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms.PathFinding;
using System;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorGenericSwitch : IFurniInteractor
    {
        public void OnPlace(GameClient session, Item item)
        {
        }

        public void OnRemove(GameClient session, Item item)
        {
        }

        public void OnTrigger(GameClient session, Item item, int request, bool hasRights)
        {
            int modes = item.GetBaseItem().Modes - 1;

          
            var This = session.GetRolePlay();

            #region Trash Bin
            if (item.BaseItem == 20013)
            {
                if (!This.onduty)
                {
                    int Distance = Math.Abs(item.GetX - This.roomUser.X) + Math.Abs(item.GetY - This.roomUser.Y);
                    if (Distance <= 1 || This.CheckDiag(item.GetX, item.GetY, This.roomUser.X, This.roomUser.Y))
                    {
                        This.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, item.GetX, item.GetY), false);
                        if (This.InteractingItem == item.Id)
                        {
                            session.SendWhisper("This item is already being searched!");
                            return;
                        }
                        if (This.Dead || This.Energy < 1 || This.Cuffed || This.Health < 1 || This.Cooldown3 > 0)
                        {
                            This.Responds();
                            return;
                        }
                        if (This.InteractingCD > 0)
                        {
                            session.SendWhisper("This ability is on cooldown (" + This.InteractingCD + ")");
                            return;
                        }
                        if (This.InteractingItem == 0)
                        {
                            This.InteractingItem = item.BaseItem;
                            This.Say("begins to search through the trash can", true, 4);
                            This.roomUser.ApplyEffect(10);
                            item.ExtraData = "1";
                            item.UpdateState();
                            This.Cooldown3 = 10;
                        }
                        This.roomUser.ClearMovement(true);
                    }
                    else This.roomUser.MoveTo(item.SquareInFront);
                    return;
                }
            }
            #endregion

            #region Farm
            if (item.BaseItem == 4552) // carrot
            {
                int Distance = Math.Abs(item.GetX - This.roomUser.X) + Math.Abs(item.GetY - This.roomUser.Y);
                if (Distance <= 1 || This.CheckDiag(item.GetX, item.GetY, This.roomUser.X, This.roomUser.Y))
                {
                    This.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, item.GetX, item.GetY), false);
                    if (This.Inventory.IsInventoryFull("carrot"))
                    {
                        This.GetClient().SendWhisper("Your inventory is currently full!");
                        return;
                    }
                    This.Room.GetRoomItemHandler().RemoveRoomItem(item, This.habbo.Id);
                    This.Inventory.Additem("carrot");
                    This.Say("plucks a fresh carrot from the ground", false);
                    This.roomUser.ClearMovement(true);
                    return;
                }

            }
            if (item.BaseItem == 10000052) // weed
            {
                int Distance = Math.Abs(item.GetX - This.roomUser.X) + Math.Abs(item.GetY - This.roomUser.Y);
                if (Distance <= 1 || This.CheckDiag(item.GetX, item.GetY, This.roomUser.X, This.roomUser.Y))
                {
                    This.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, item.GetX, item.GetY), false);
                    if (This.Inventory.IsInventoryFull("weed"))
                    {
                        This.GetClient().SendWhisper("Your inventory is currently full!");
                        return;
                    }
                    This.Room.GetRoomItemHandler().RemoveRoomItem(item, This.habbo.Id);
                    This.Inventory.Additem("weed");
                    This.Say("plucks a weed from the ground", false);
                    This.roomUser.ClearMovement(true);
                    return;
                }

            }
            if (item.BaseItem == 20094) // dirt nest
            {
                /*int Distance = Math.Abs(Item.GetX - This.roomUser.X) + Math.Abs(Item.GetY - This.roomUser.Y);
                if (Distance <= 1 || This.CheckDiag(Item.GetX, Item.GetY, This.roomUser.X, This.roomUser.Y))
                {
                    This.SetRot(Rotation.Calculate(This.roomUser.X, This.roomUser.Y, Item.GetX, Item.GetY), false);
                    if (This.equip1 != "seed")
                    {
                        Session.SendWhisper("You must first equip a plant seed before performing this action!");
                        return;
                    }
                    foreach (RoomUser User in This.Room.GetRoomUserManager().GetRoomUsers())
                        if (User.GetClient().GetRolePlay().PlantSeed == Item.Id && User.GetClient().GetRolePlay().Seed > 0)
                        {
                            Session.SendWhisper("This dirt nest is currently being used by another user!");
                            return;
                        }
                    if (This.PlantSeed > 0)
                    {
                        Session.SendWhisper("You can only plant one seed at a time!");
                        return;
                    }
                    This.roomUser.CanWalk = false;
                    This.PlantSeed = Item.Id;
                    This.PlantSeedRoom = This.Room.Id;
                    This.SendWeb("{\"name\":\"farm\"}");
                    This.roomUser.ClearMovement(true);
                    return;
                }
                */
            }
            #endregion
            if (session == null || !hasRights || modes <= 0)
            {
                return;
            }
            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniSwitch);

            int currentMode = 0;
            int newMode = 0;

            if (!int.TryParse(item.ExtraData, out currentMode))
            {
            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            item.ExtraData = newMode.ToString();
            item.UpdateState();
        }

        public void OnWiredTrigger(Item item)
        {
            int modes = item.GetBaseItem().Modes - 1;

            if (modes == 0)
            {
                return;
            }

            int currentMode = 0;
            int newMode = 0;

            if (string.IsNullOrEmpty(item.ExtraData))
                item.ExtraData = "0";

            if (!int.TryParse(item.ExtraData, out currentMode))
            {
                return;
            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            item.ExtraData = newMode.ToString();
            item.UpdateState();
        }
    }
}