import * as _ from "lodash";

import { GameWrap } from "./GameWrap";

export class Site
{
    readonly site: ConstructionSite;

    get id(): Id<ConstructionSite> { return this.site.id; }
    get type(): BuildableStructureConstant { return this.site.structureType; }
    get pos(): RoomPosition { return this.site.pos; }

    get progress(): number { return this.site.progress; }
    get total(): number { return this.site.progressTotal; }
    get remaining(): number { return this.total - this.progress; }

    constructor(site: ConstructionSite)
    {
        this.site = site;
    }
}

export class Sites
{
    private static _all: Site[] = [];
    private static _notWalls: Site[] = [];
    private static _byId: { [id: Id<ConstructionSite>]: Site } = {};

    static get(id: Id<ConstructionSite> | undefined): Site | undefined { return id ? Sites._byId[id] : undefined; }

    static get all(): Site[] { return Sites._all; }
    static get notWalls(): Site[] { return Sites._notWalls; }

    static initialize()
    {
        Sites._all = GameWrap.myConstructionSites.map(s => new Site(s));
        Sites._notWalls = Sites._all.filter(s => s.type != STRUCTURE_WALL);
        Sites._byId = _.indexBy(Sites._all, "id");
    }
}