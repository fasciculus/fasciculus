import { Cached } from "./screeps.util";
import { Objects } from "./types.util";

export class ScreepsSite
{
    private static progressRemaining(this: ConstructionSite): number
    {
        return this.progressTotal - this.progress;
    }

    private static _my: Cached<Map<SiteId, ConstructionSite>> = Cached.simple(ScreepsSite.fetchMy);

    private static fetchMy(): Map<SiteId, ConstructionSite>
    {
        return Objects.values(Game.constructionSites).indexBy(s => s.id);
    }

    private static my(): Array<ConstructionSite>
    {
        return ScreepsSite._my.value.vs();
    }

    private static myIds(): Set<SiteId>
    {
        return ScreepsSite._my.value.ks();
    }

    static setup()
    {
        Objects.setGetter(ConstructionSite.prototype, "progressRemaining", ScreepsSite.progressRemaining);

        Objects.setGetter(ConstructionSite, "my", ScreepsSite.my);
        Objects.setGetter(ConstructionSite, "myIds", ScreepsSite.myIds);
    }
}