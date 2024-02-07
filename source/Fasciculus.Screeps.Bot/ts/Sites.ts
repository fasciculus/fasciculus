
import { profile } from "./Profiling";

export class Site
{
    readonly id: SiteId;

    get site(): ConstructionSite { return Game.get<ConstructionSite>(this.id)!; }

    get pos(): RoomPosition { return this.site.pos; }

    get remaining(): number { return this.site.progressRemaining; }

    constructor(id: SiteId)
    {
        this.id = id;
    }
}

export class Sites
{
    private static _sites: Map<SiteId, Site> = new Map();

    static get(id: SiteId | undefined): Site | undefined { return id ? Sites._sites.get(id) : undefined; }

    static get all(): Array<Site> { return Sites._sites.vs(); }
    static get count(): number { return Sites._sites.size; }

    @profile
    static initialize()
    {
        Sites._sites.update(ConstructionSite.myIds, id => new Site(id));
    }
}