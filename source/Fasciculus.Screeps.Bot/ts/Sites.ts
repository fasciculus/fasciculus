
import { Dictionaries, Dictionary, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chambers } from "./Rooms";

export class Site
{
    readonly id: SiteId;

    get site(): ConstructionSite { return Game.get<ConstructionSite>(this.id)!; }

    get type(): BuildableStructureConstant { return this.site.structureType; }
    get pos(): RoomPosition { return this.site.pos; }

    get progress(): number { return this.site.progress; }
    get total(): number { return this.site.progressTotal; }
    get remaining(): number { return this.total - this.progress; }

    constructor(id: SiteId)
    {
        this.id = id;
    }
}

export class Sites
{
    private static _sites: Map<SiteId, Site> = new Map();

    static get(id: SiteId | undefined): Site | undefined { return id ? Sites._sites.get(id) : undefined; }

    static get all(): Vector<Site> { return Vector.from(Sites._sites.vs()); }
    static get count(): number { return Sites._sites.size; }

    @profile
    static initialize()
    {
        if (Sites._sites.update(Game.mySiteIds, id => new Site(id)))
        {
            Chambers.reset();
        }
    }
}