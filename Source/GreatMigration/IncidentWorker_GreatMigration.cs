using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
using UnityEngine;

namespace GreatMigration
{
    class IncidentWorker_GreatMigration : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            IntVec3 intVec;
            if (!RCellFinder.TryFindRandomPawnEntryCell(out intVec))
            {
                return false;
            }
            PawnKindDef pawnKindDef = (
                from a in Find.Map.Biome.AllWildAnimals
                where GenTemperature.SeasonAcceptableFor(a.race)
                select a).RandomElement();
            float points = IncidentMakerUtility.DefaultParmsNow(Find.Storyteller.def, IncidentCategory.ThreatBig).points;
            int num = Rand.RangeInclusive(12, 24);
            int num2 = Rand.RangeInclusive(90000, 150000);
            IntVec3 invalid = IntVec3.Invalid;
            if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(intVec, 10f, out invalid))
            {
                invalid = IntVec3.Invalid;
            }
            Pawn pawn = null;
            for (int i = 0; i < num; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, 10);
                pawn = PawnGenerator.GeneratePawn(pawnKindDef, null);
                GenSpawn.Spawn(pawn, loc, Rot4.Random);
                pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num2;
                if (invalid.IsValid)
                {
                    pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, 10);
                }
            }
            Find.LetterStack.ReceiveLetter("Migration", "A group of animals is migrating through your region.", LetterType.Good, intVec, null);
            return true;
        }
    }
}
