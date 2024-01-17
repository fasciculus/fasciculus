import { Rooms } from "./Rooms";

export class Controllers
{
    private static _all: StructureController[] = [];
    private static _my: StructureController[] = [];

    static get all(): StructureController[] { return Controllers._all; }
    static get my(): StructureController[] { return Controllers._my; }

    static initialize()
    {
        Controllers._all = Rooms.all.map(r => r.controller).filter(c => c) as StructureController[];
        Controllers._my = Controllers._all.filter(c => c.my);
    }
}