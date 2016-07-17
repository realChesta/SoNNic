using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SonicPlugin.Sonic;
using BizHawk.Emulation.Common;

namespace SonicPlugin.Sonic
{
    public class SonicObject
    {
        public const long ReservedObjectsOffset = 0xD000;
        public const long MainObjectsOffset = 0xD800;
        public const long ObjectSectionEnd = 0xEFFF;
        public const long ObjectCollisionTable = 0x1AE2E - 2; //because 0x00 is ignored

        public SonicObjectType ObjectType;
        public byte ObjectSubType;
        public RenderFlags Flags;
        //public ArtTile ArtTile;

        public uint MappingOffset;

        public ushort Position_X;
        public ushort Position_Y;

        public ushort Velocity_X;
        public ushort Velocity_Y;
        public ushort Velocity_Ground;

        public byte Hitbox_VerticalRadius;
        public byte Hitbox_HorizontalRadius;

        public byte NewHitbox_VerticalRadius;
        public byte NewHitbox_HorizontalRadius;

        public ObjectStatus Status;

        public CollisionResponseType CollisionResponse;

        public ushort Angle;

        public SonicObject(IMemoryDomains domains, long offset)
        {
            this.ObjectType = (SonicObjectType)domains.MainMemory.PeekByte(offset);

            if (this.ObjectType == SonicObjectType.Null)
                return;

            this.Flags = new RenderFlags(domains.MainMemory.PeekByte(offset + 0x01));
            //this.ArtTile = new ArtTile(new byte[] { memory.MainMemory.PeekByte(offset + 0x02), memory.MainMemory.PeekByte(offset + 0x03) });

            this.MappingOffset = domains.MainMemory.PeekDWord(offset + 0x04, true);

            this.Position_X = domains.MainMemory.PeekWord(offset + 0x08, true);
            this.Position_Y = domains.MainMemory.PeekWord(offset + 0x0C, true);

            this.Velocity_X = domains.MainMemory.PeekWord(offset + 0x10, true);
            this.Velocity_Y = domains.MainMemory.PeekWord(offset + 0x12, true);
            this.Velocity_Ground = domains.MainMemory.PeekWord(offset + 0x14, true);

            this.Hitbox_VerticalRadius = domains.MainMemory.PeekByte(offset + 0x16);
            this.Hitbox_HorizontalRadius = domains.MainMemory.PeekByte(offset + 0x17);

            byte collisionReponseByte = domains.MainMemory.PeekByte(offset + 0x20);

            if (ObjectType != SonicObjectType.Spikes)
                this.CollisionResponse = Utils.GetCollisionResponseType(collisionReponseByte);
            else
                this.CollisionResponse = CollisionResponseType.Harm;

            this.ObjectSubType = (ObjectType == SonicObjectType.Spikes) ? domains.MainMemory.PeekByte(offset + 0x1A) : domains.MainMemory.PeekByte(offset + 0x28);                

            LookupSize(domains, collisionReponseByte);

            //this.Status = new ObjectStatus(memory.MainMemory.PeekByte(offset + 0x22)); //TODO: uncomment if needed

            this.Angle = domains.MainMemory.PeekWord(offset + 0x26, true);
        }

        public static List<SonicObject> ReadObjects(IMemoryDomains memory, bool includeReserved)
        {
            List<SonicObject> objs = new List<SonicObject>();

            long offset = includeReserved ? ReservedObjectsOffset : MainObjectsOffset;

            for (; offset < ObjectSectionEnd; offset += 0x40)
            {
                SonicObject so = new SonicObject(memory, offset);

                if (so.ObjectType != SonicObjectType.Null)
                    objs.Add(so);
            }

            return objs;
        }

        public void LookupSize(IMemoryDomains domains, byte collisionReponseByte)
        {
            switch (ObjectType)
            {
                case SonicObjectType.Spikes:
                    {
                        switch (ObjectSubType)
                        {
                            case 0:
                                this.NewHitbox_HorizontalRadius = 15;
                                this.NewHitbox_VerticalRadius = 16;
                                break;
                            case 1:
                                this.NewHitbox_HorizontalRadius = 14;
                                this.NewHitbox_VerticalRadius = 4;
                                break;
                            case 2:
                                this.NewHitbox_HorizontalRadius = 7;
                                this.NewHitbox_VerticalRadius = 16;
                                break;
                            case 3:
                                this.NewHitbox_HorizontalRadius = 20;
                                this.NewHitbox_VerticalRadius = 16;
                                break;
                            case 4:
                                this.NewHitbox_HorizontalRadius = 37;
                                this.NewHitbox_VerticalRadius = 16;
                                break;
                            case 5:
                                this.NewHitbox_HorizontalRadius = 13;
                                this.NewHitbox_VerticalRadius = 20;
                                break;
                        }
                        //this.NewHitbox_HorizontalRadius = 20;
                        //this.NewHitbox_VerticalRadius = 16;
                    }
                    break;

                case SonicObjectType.Spring:
                {
                    this.NewHitbox_HorizontalRadius = 15;
                    this.NewHitbox_VerticalRadius = 8;
                }
                break;

                case SonicObjectType.GHZBridge:
                {
                    this.NewHitbox_HorizontalRadius = 8;
                    this.NewHitbox_VerticalRadius = 8;
                }
                break;

                case SonicObjectType.PurpleRock:
                {
                    this.NewHitbox_HorizontalRadius = 14;
                    this.NewHitbox_VerticalRadius = 16;
                }
                break;

                case SonicObjectType.BreakableWall:
                {
                    this.NewHitbox_HorizontalRadius = 14;
                    this.NewHitbox_VerticalRadius = 32;
                }
                break;

                case SonicObjectType.CollapsingLedge:
                {
                    this.NewHitbox_HorizontalRadius = 48;
                    int index = (domains.MainMemory.PeekWord(0xD008, true) - this.Position_X + this.NewHitbox_HorizontalRadius);

                    if (this.Flags.HorizontalMirror)
                        index = (this.NewHitbox_HorizontalRadius * 2) - index;

                    if (index < 0 || index > (this.NewHitbox_HorizontalRadius * 2))
                        break;


                    this.NewHitbox_VerticalRadius = (byte)(domains["MD CART"].PeekByte(0x8570 + index / 2));
                }
                break;

                case SonicObjectType.SwingingPlatformOrSpikedBall:
                {
                    this.NewHitbox_HorizontalRadius = 24;
                    this.NewHitbox_VerticalRadius = 8;
                }
                break;

                case SonicObjectType.Platform:
                {
                    this.NewHitbox_HorizontalRadius = 32;
                    //this.NewHitbox_VerticalRadius = 8;
                }
                break;

                default:
                    {
                        byte collisionIndex = (byte)(collisionReponseByte & 0x3F);
                        if (collisionIndex != 0x00)
                        {
                            //System.Windows.Forms.MessageBox.Show("collisionIndex: " + collisionIndex + ", horiz-addr: 0x" + (ObjectCollisionTable + (collisionIndex * 2)).ToString("X2"));
                            long table = ObjectCollisionTable + (collisionIndex * 2);
                            this.NewHitbox_HorizontalRadius = domains["MD CART"].PeekByte(table);
                            this.NewHitbox_VerticalRadius = domains["MD CART"].PeekByte(table + 1);
                            //log.WriteLine("tablePointer: 0x" + table.ToString("X2") + "\nX: " + NewHitbox_HorizontalRadius + ", Y: " + NewHitbox_VerticalRadius);
                        }
                        else
                        {
                            this.NewHitbox_HorizontalRadius = Hitbox_HorizontalRadius;
                            this.NewHitbox_VerticalRadius = Hitbox_VerticalRadius;
                        }
                    }
                    break;
            }
        }
    }
}
