import * as _ from "lodash";
import { Chamber, Chambers } from "./Chambers";
import { Memories, WellMemory } from "./Memories";
import { DIRECTIONS, Point } from "./Geometry";
import { Creeps } from "./Creeps";
import { Utils } from "./Utils";
import { Bodies } from "./Bodies";
import { Vectors } from "./Collections";

export class Well
{
    readonly source: Source;

    get id(): Id<Source> { return this.source.id; }
    get memory(): WellMemory { return Memories.well(this.id); }

    get pos(): RoomPosition { return this.source.pos; }
    get room(): Room { return this.source.room; }
    get chamber(): Chamber | undefined { return Chambers.get(this.room.name); }

    get energy(): number { return this.source.energy; }
    get energyCapacity(): number { return this.source.energyCapacity; }

    get slots(): number
    {
        let result = this.memory.slots;

        if (!result)
        {
            this.memory.slots = result = this.countSlots();
        }

        return result;
    }

    get assignees(): Creep[] { return Utils.defined((this.memory.assignees || []).map(n => Creeps.get(n))); }
    private set assignees(value: Creep[]) { this.memory.assignees = value.map(c => c.name); }
    get freeSlots(): number { return this.slots - this.assignees.length; }
    get assignedWork(): number { return _.sum(this.assignees.map(Bodies.workOf)); }
    get maxWork(): number { return this.energyCapacity / 300; }
    get unassignedWork(): number { return this.maxWork - this.assignedWork; }
    get assignable(): boolean { return this.freeSlots > 0 && this.unassignedWork > 0; }

    constructor(source: Source)
    {
        this.source = source;
    }

    assign(creep: Creep)
    {
        let assignees = this.assignees;

        assignees.push(creep);

        this.assignees = assignees;
    }

    private countSlots(): number
    {
        let territory = this.chamber?.territory;

        if (!territory) return 0;

        let types = territory.around(Point.from(this.pos));

        return DIRECTIONS.filter(d => types.at(d) == 0).length;
    }
}

export class Wells
{
    private static _all: Well[] = [];
    private static _byId: { [id: Id<Source>]: Well } = {};

    static get(id: Id<Source> | undefined): Well | undefined { return id ? Wells._byId[id] : undefined; }

    static get assignable(): Well[] { return Wells._all.filter(w => w.assignable); }

    static get maxEnergyPerTick(): number { return _.sum(Wells._all.map(w => w.energyCapacity)) / 300; }

    static initialize()
    {
        let sources = Vectors.flatten(Chambers.all.map(c => c.sources));

        Wells._all = sources.map(s => new Well(s)).values;
        Wells._byId = _.indexBy(Wells._all, w => w.id);
    }
}