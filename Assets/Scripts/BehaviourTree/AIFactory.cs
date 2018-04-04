using System;
using Assets.Scripts.BehaviourTree.Branch;
using Assets.Scripts.BehaviourTree.Decorator;
using Assets.Scripts.BehaviourTree.Leaf.Actions;
using Assets.Scripts.BehaviourTree.Leaf.Conditions;
using Assets.Scripts.Points;

namespace Assets.Scripts.BehaviourTree
{
    /// <summary>
    /// Factory pattern. Creates and returns behaviour trees 
    /// for different circumstances/difficulty levels. 
    /// </summary>
    public class AIFactory
    {
        #region Singleton

        private static readonly AIFactory _instance = new AIFactory();

        public static AIFactory Instance
        {
            get { return _instance; }
        }

        #endregion

        /// <summary>
        /// Creates a behaviour tree for the specified difficulty level
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="dataModel"></param>
        /// <returns></returns>
        public BT CreateBehaviourTree(Difficulty difficulty, DataModel dataModel)
        {
            switch (difficulty)
            {
                case Difficulty.None:
                    return CreateNoneBT(dataModel);
                case Difficulty.Plein:
                    return CreatePleinBT(dataModel);
                case Difficulty.Street:
                    return CreateStreetBT(dataModel);
                case Difficulty.Easy:
                    return CreateEasyBT(dataModel);
                case Difficulty.Medium:
                    throw new NotImplementedException("Medium difficulty is not implemented.");
                case Difficulty.Hard:
                    throw new NotImplementedException("Hard difficulty is not implemented.");
                case Difficulty.Door:
                    return CreateDoorBT(dataModel);
                default:
                    throw new ArgumentOutOfRangeException(
                        "difficulty", difficulty, "Unsupported difficulty level. Cannot create BT.");
            }
        }

        private static BT CreateNoneBT(DataModel d)
        {
            return new BT(
                new Sequence
                {
                    new While(
                        new Sequence
                        {
                            new IsHostile(d),
                        },
                        new Selector{
                             new IsWeaponEquipped(d),
                                new Sequence
                                {
                                    new EquipRandomWeapon(d),
                                    new Wait(3f)
                                }
                        }
                      )
                });

        }

        private static BT CreateStreetBT(DataModel d)
        {
            Sequence rootSelector = new Sequence
            {
                new While(new IsPanicking(d), //panic
                    
                    new Sequence //run away
                    {
                        new SetMovementSpeed(d, true),
                        new SetTarget(d, PointType.Despawn, PointList.GetSafestPoint),
                        new Succeeder(new Seek(d, x => x.Target)),
                        new Despawn(d)
                    }
                ),
                new Sequence
                {
                    //Attack
                    new While( //Continues as long as everthing returns true
                        new Sequence //Step 1. If NPC is hostile continue
                        {
                            new IsHostile(d),
                            new CanAttack(d)
                        },
                        new Sequence //Step 2, starting attack
                        {
                            new SetMovementSpeed(d, true), //Allows npc to run
                            new RandomSelector { //Selects NPC or Player at random.
                                new SetTarget(d),
                                new SetTarget(d,true)
                            },
                            new Selector //Equiping a weapon
                            {
                                new IsWeaponEquipped(d), //If NPC has a weapon, do nothing else equip
                                new Sequence
                                {
                                    new EquipRandomWeapon(d),
                                    new Wait(3f)
                                }
                            },
                            new While( //movetotarget
                                new Sequence
                                {
                                    new IsWithinWeaponsRange(d, true),
                                    new CanSeeTarget(d, true)
                                },
                            new Seek(d, x => x.Target) //If the target is not visable, seek the target.
                            ),
                            new While(
                                new Sequence //use conditions
                                {
                                    new IsWithinWeaponsRange(d),
                                    new IsTargetAlive(d),
                                    new TurnToFaceTarget(d)
                                }, 
                                new Repeater( //use weapon
                                    new Sequence
                                    {
                                        new ReloadWeapon(d),
                                        new CausePanic(d),
                                        new UseItem(d),
                                        new Wait(1f)
                                    }, true)
                            )
                        }

                    )
                },
               new Sequence
                {
                    //Wander
                    new While(
                        new Sequence{
                            new RandomSelector
                            {
                                new Sequence //wandersteer
                                {
                                    new SetMovementSpeed(d, false),
                                    new Wander(d),
                                    new RandomWait(0.5f, 1.0f)
                                },
                                new Sequence //move to poi
                                {
                                    new SetMovementSpeed(d, false),
                                    new SetTarget(d, PointType.Interest, PointList.GetRandomPoint),
                                    new Seek(d, x => x.Target),
                                    new RandomWait(0.5f, 1.0f)
                                }
                            }
                            },
                        new Sequence{})
                        }          
            };
            return new BT(rootSelector);
        }

        private static BT CreatePleinBT(DataModel d)
        {
            // -- Root --
            Sequence rootSelector = new Sequence
            {
                //Attack
                new Sequence
                {
                    new While(
                        new Sequence
                        {
                            new IsHostile(d),
                            new CanAttack(d)
                        },
                        new Sequence
                        {
                            new SetMovementSpeed(d, true),
                            new RandomSelector {
                                new SetTarget(d),
                                new SetTarget(d,true)
                            },
                            new Selector
                            {
                                // Equip
                                new IsWeaponEquipped(d),
                                new Sequence
                                {
                                    new EquipRandomWeapon(d),
                                    new Wait(3f)
                                }
                            },
                            new While( //movetotarget
                                new Sequence
                                {
                                    new IsWithinWeaponsRange(d, true),
                                    new CanSeeTarget(d, true)
                                }, new Seek(d, x => x.Target)),
                            new While(
                                new Sequence //use conditions
                                {
                                    new IsWithinWeaponsRange(d),
                                    new IsTargetAlive(d),
                                    new TurnToFaceTarget(d)
                                }, new Repeater( //use weapon
                                    new Sequence
                                    {
                                        new ReloadWeapon(d),
                                        new CausePanic(d),
                                        new UseItem(d),
                                        new Wait(1f)
                                    }, true)
                            )
                        }
                    )
                },
                new While(new IsPanicking(d), //panic
                    new RandomSelector
                    {
                        new Sequence //run away
                        {
                            new SetMovementSpeed(d, true),
                            new SetTarget(d, PointType.Despawn, PointList.GetSafestPoint),
                            new Succeeder(new Seek(d, x => x.Target, 3f)),
                            new Despawn(d)
                        },
                        new Sequence //freeze
                        {
                            new TriggerAnimation(d, "Nervous"),
                            new RandomWait(2, 6),
                            new TriggerAnimation(d, "Idle")
                        }
                    }
                ),
                new While(//wander
                    new Sequence
                    {
                        //new IsHostile(d, true),
                        new IsPanicking(d, true),
                        new Inverter(new Sequence()
                        {
                            new IsHostile(d),
                            new CanAttack(d)
                        })

                    },
                    new RandomSelector
                    {
                        new Sequence //wandersteer
                        {
                            new SetMovementSpeed(d, false),
                            new Wander(d),
                            new RandomWait(0.5f, 2.0f)
                        },
                        new Sequence //move to poi
                        {
                            new SetMovementSpeed(d, false),
                            new SetTarget(d, PointType.Interest, PointList.GetRandomPoint),
                            new Seek(d, x => x.Target),
                            new RandomWait(0.5f, 1.0f)
                        }
                    }
                )
            };

            return new BT(rootSelector);
        }

        private static BT CreateEasyBT(DataModel d)
        {
            return new BT(new Selector
            {
                //attack
                new Sequence
                {
                    new IsHostile(d),
                    new CanAttack(d),
                    new SetTarget(d, true),
                    new EquipWeapon(d),
                    new TurnToFaceTarget(d),
                    new Wait(2f),
                    new UseItem(d),
                    new RandomWait(0.5f, 1.5f)
                },
                //panic
                new Sequence
                {
                    new IsHostile(d, true),
                    new CanAttack(d),
                    new SetMovementSpeed(d, true),
                    new Wander(d)
                },
                //wander    
                new While(new CanAttack(d, true),
                    new Sequence
                    {
                        new SetMovementSpeed(d, false),
                        new SetTargetWaypoint(d),
                        new Seek(d, x => x.MovePosition),
                        new RandomWait(0.5f, 1)
                    }
                )
            });
        }


        private static BT CreateDoorBT(DataModel d)
        {
            return new BT(new Selector
            {
                // Attack
                new Sequence
                {
                    new IsDoorOpen(d),
                    new IsHostile(d),

                    // Point weapon at either the player or another NPC
                    new ExecuteOnce(new Sequence
                    {
                        new Wait(0.3f),
                        new EquipWeapon(d),
                        new Wait(1f),
                        new RandomSelector
                        {
                           // new SetTarget(d),
                            new SetTarget(d, true)
                        },
                        new TurnToFaceTarget(d),
                        new SetTarget(d, true),
                    }),

                    new CanSeeTarget(d),
                    new TurnToFaceTarget(d, 120f),

                    // Attack the player
                    new While(new Sequence()
                    {
                        new IsTargetAlive(d),
                        new CanSeeTarget(d)
                    },
                        new Sequence()
                        {
                            // Attack
                            new TurnToFaceTarget(d),
                            new Wait(0.5f),
                            new UseItem(d),

                        }
                    )
                }
            });
        }
    }
}