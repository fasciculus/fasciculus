import * as _ from "lodash";

import { GameWrap } from "./GameWrap";
import { Dictionary, Vector } from "./Collections";
import { SiteId } from "./Types";

export class Site
{
    readonly site: ConstructionSite;

    get id(): SiteId { return this.site.id; }
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
    private static _all: Vector<Site> = new Vector();
    private static _notWalls: Vector<Site> = new Vector();
    private static _byId: Dictionary<Site> = {};

    static get(id: SiteId | undefined): Site | undefined { return id ? Sites._byId[id] : undefined; }

    static get all(): Vector<Site> { return Sites._all.clone(); }
    static get notWalls(): Vector<Site> { return Sites._notWalls.clone(); }

    static initialize()
    {
        Sites._all = GameWrap.myConstructionSites.map(s => new Site(s));
        Sites._notWalls = Sites._all.filter(s => s.type != STRUCTURE_WALL);
        Sites._byId = Sites._all.indexBy(s => s.id);
    }
}