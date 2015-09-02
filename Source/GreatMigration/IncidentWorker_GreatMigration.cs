using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;
using RimWorld.SquadAI;

namespace GreatMigration
{
    class IncidentWorker_GreatMigration : IncidentWorker
    {
        private Job job1 = new Job();
        private Job job2 = new Job();
        public override bool TryExecute(IncidentParms parms)
        {
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec))
            {
                return false;
            }
            if (SpawnRandomMigrationAt(intVec))
            {
                Find.LetterStack.ReceiveLetter("Migration", "A group of animals is migrating through your region.", LetterType.Good, intVec, null);
                return true;
            }
            return false;           
        }

        public bool checkDistance(IntVec3 cell1, IntVec3 cell2)
        {
            return (cell1 - cell2).ToVector3().magnitude > 50;
        }

        public bool SpawnRandomMigrationAt(IntVec3 loc)
        {
            PawnKindDef pawnKindDef = (
                from a in Find.Map.Biome.AllWildAnimals
                where GenTemperature.SeasonAcceptableFor(a.race)
                select a).RandomElement();
            if (pawnKindDef == null)
            {
                Log.Error("No spawnable animals right now.");
                return false;
            }
            int randomInRange = Rand.RangeInclusive(12, 24);
            IntVec3 enterCell = loc;
            IntVec3 exitCell = CellFinder.RandomEdgeCell();
            if (checkDistance(enterCell, exitCell))
            {
                for (int i = 0; i < randomInRange; i++)
                {
                    Pawn newThing = PawnGenerator.GeneratePawn(pawnKindDef, null, false, 0);
                    IntVec3 groupCell = CellFinder.RandomClosewalkCellNear(enterCell, 10);
                    GenSpawn.Spawn(newThing, groupCell);
                    job1 = new Job(JobDefOf.Goto, exitCell)
                    {
                        exitMapOnArrival = true
                    };
                    newThing.jobs.StartJob(job1, JobCondition.None, null, false, true);
                    newThing.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(10000, 12000);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
