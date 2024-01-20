import * as _ from "lodash";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { Bodies } from "./Bodies";
import { RepairerMemory } from "./Memories";
import { Repairable } from "./Types";
import { Repairs } from "./Repairs";
import { Walls } from "./Walls";

const REPAIRER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#0f0"
    }
};

class DamageHelper
{
    static damageOf(repairable: Repairable): number
    {
        let hitsMax = repairable instanceof StructureWall ? Walls.avg + 2000 : repairable.hitsMax;

        return Math.max(0, hitsMax - repairable.hits);
    }
}

export class Repairer extends CreepBase
{
    get memory(): RepairerMemory { return super.memory as RepairerMemory; }

    get repairable(): Repairable | undefined { return Repairs.get(this.memory.repairable); }
    set repairable(value: Repairable | undefined) { this.memory.repairable = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToRepairable: this.executeToRepair(); break;
            case CreepState.Repair: this.executeRepair(); break;
        }
    }

    private executeToRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.moveTo(repairable, REPAIRER_MOVE_TO_OPTS);
    }

    private executeRepair()
    {
        let repairable = this.repairable;

        if (!repairable) return;

        this.repair(repairable);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToRepairable: this.state = this.prepareToRepairable(); break;
            case CreepState.Repair: this.state = this.prepareRepair(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareToRepairable(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        return this.inRangeTo(repairable) ? CreepState.Repair : CreepState.ToRepairable;
    }

    private prepareRepair(): CreepState
    {
        let repairable = this.repairable;

        if (!repairable) return CreepState.Idle;

        if (DamageHelper.damageOf(repairable) == 0)
        {
            this.repairable = undefined;
            return CreepState.Idle;
        }

        return CreepState.Repair;
    }

    private inRangeTo(repairable: Repairable)
    {
        return this.pos.inRangeTo(repairable, 2);
    }
}
 
const REPAIRER_PARTS: BodyPartConstant[] = [WORK, CARRY, MOVE, WORK, CARRY, MOVE, WORK, CARRY, MOVE, WORK, CARRY, MOVE, WORK, CARRY, MOVE];
const REPAIRER_MIN_SIZE = 3;

interface DamageInfo
{
    repairable: Repairable;
    damage: number;
}

export class Repairers
{
    private static _all: Repairer[] = [];

    static get all(): Repairer[] { return Repairers._all; }

    static initialize()
    {
        Repairers._all = Creeps.ofType(CreepType.Repairer).map(c => new Repairer(c));

        Bodies.register(CreepType.Repairer, REPAIRER_MIN_SIZE, REPAIRER_PARTS);
    }

    static run()
    {
        Repairers._all.forEach(r => r.prepare());

        let assigned = Repairers.assign();

        assigned.forEach(r => r.prepare());
        Repairers._all.forEach(r => r.execute());
    }

    private static assign(): Repairer[]
    {
        var result: Repairer[] = [];
        var unassigned: _.Dictionary<Repairer> = Repairers.unassignedRepairers;

        if (_.size(unassigned) == 0) return result;

        let repairables = Repairers.repairabes.sort(Repairers.compareDamage);

        for (let i = 0, n = repairables.length; i < n; ++i)
        {
            let repairable = repairables[i].repairable;
            let assignables: Repairer[] = _.values(unassigned);
            let repairer = repairable.pos.findClosestByPath(assignables) || undefined;

            if (!repairer) continue;

            repairer.repairable = repairable;
            result.push(repairer);
            delete unassigned[repairer.name];

            if (_.size(unassigned) == 0) break;
        }

        return result;
    }

    private static get unassignedRepairers(): _.Dictionary<Repairer>
    {
        let unassigned = Repairers.all.filter(r => !r.repairable);

        return _.indexBy(unassigned, r => r.name);
    }

    private static get repairabes(): DamageInfo[]
    {
        return Repairs.all.map(Repairers.toDamageInfo);
    }

    private static toDamageInfo(repairable: Repairable): DamageInfo
    {
        let result: DamageInfo = { repairable, damage: DamageHelper.damageOf(repairable) };

        return result;
    }

    private static compareDamage(a: DamageInfo, b: DamageInfo): number
    {
        return b.damage - a.damage;
    }
}