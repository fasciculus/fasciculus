
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
    private static _sites: Dictionary<Site> = {};
    private static _count: number = 0;

    static get(id: SiteId | undefined): Site | undefined { return id ? Sites._sites[id] : undefined; }

    static get all(): Vector<Site> { return Dictionaries.values(Sites._sites); }
    static get count(): number { return Sites._count; }

    @profile
    static initialize(reset: boolean)
    {
        if (reset)
        {
            Sites._sites = {};
            Sites._count = 0;
        }

        const existing: Set<string> = Game.mySiteIds;

        if (Dictionaries.update(Sites._sites, existing, id => new Site(id as SiteId)))
        {
            Sites._count = Dictionaries.size(Sites._sites);
            Chambers.reset();
        }
    }
}