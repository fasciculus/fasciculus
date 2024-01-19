import * as _ from "lodash";
import { Chamber, Chambers } from "./Chambers";
import { Memories, WellMemory } from "./Memories";
import { DIRECTIONS, Point } from "./Geometry";

export class Well
{
    private _slots?: DirectionConstant[];

    readonly source: Source;

    get id(): Id<Source> { return this.source.id; }
    get memory(): WellMemory { return Memories.well(this.id); }

    get pos(): RoomPosition { return this.source.pos; }
    get room(): Room { return this.source.room; }
    get chamber(): Chamber | undefined { return Chambers.get(this.room.name); }

    get slots(): DirectionConstant[] { return this._slots || (this._slots = this.memory.slots || (this.memory.slots = this.findSlots())); }

    get assignees(): string[] { return this.memory.assignees || []; }
    set assignees(value: string[]) { this.memory.assignees = value; }

    get assignedWork(): number { return this.memory.assignedWork || 0; }
    set assignedWork(value: number) { this.memory.assignedWork = value; }

    get unassignedWork(): number { return Math.max(0, 5 - this.assignedWork); }

    constructor(source: Source)
    {
        this.source = source;
    }

    private findSlots(): DirectionConstant[]
    {
        let territory = this.chamber?.territory;

        if (!territory) return [];

        let types = territory.around(Point.from(this.pos));

        return DIRECTIONS.filter(d => types[d] == 0);
    }
}

export class Wells
{
    private static _all: Well[] = [];
    private static _byId: { [id: Id<Source>]: Well } = {};

    static get(id: Id<Source> | undefined): Well | undefined { return id ? Wells._byId[id] : undefined; }

    static get all(): Well[] { return Wells._all; }

    static initialize()
    {
        let sources = _.flatten(Chambers.all.map(c => c.sources));

        Wells._all = sources.map(s => new Well(s));
        Wells._byId = _.indexBy(Wells._all, w => w.id);
    }
}