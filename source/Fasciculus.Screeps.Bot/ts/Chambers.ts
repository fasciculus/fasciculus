import * as _ from "lodash";

import { GameWrap } from "./GameWrap";
import { Territories, Territory } from "./Territories";

export class Chamber
{
    private _terrain?: RoomTerrain;

    readonly room: Room;
    readonly sources: Source[];

    get name(): string { return this.room.name; }

    get terrain(): RoomTerrain { return this._terrain || (this._terrain = this.room.getTerrain()); }
    get territory(): Territory { return Territories.get(this.room); }

    constructor(room: Room)
    {
        this.room = room;
        this.sources = room.find<FIND_SOURCES, Source>(FIND_SOURCES);
    }
}

export class Chambers
{
    private static _all: Chamber[] = [];
    private static _byName: { [name: string]: Chamber } = {};

    static get(name: string | undefined): Chamber | undefined { return name ? Chambers._byName[name] : undefined; }

    static get all(): Chamber[] { return Chambers._all; }

    static initialize()
    {
        Chambers._all = GameWrap.rooms.map(r => new Chamber(r));
        Chambers._byName = _.indexBy(Chambers._all, "name");
    }
}