import { Objects } from "../es/object";
import { BodyInfos } from "./body";
import { Cached } from "./cache";
import { Names } from "./name";

export class Creeps
{
    private static _my: Cached<Map<CreepId, Creep>> = Cached.simple(Creeps.fetchMy);
    private static _byType: Cached<Map<string, Array<Creep>>> = Cached.simple(Creeps.fetchByType);

    private static type(this: Creep): string { return Names.type(this.name); }

    private static workParts(this: Creep) { return BodyInfos.workParts(this); }

    private static fetchMy(): Map<CreepId, Creep>
    {
        return Objects.values(Game.creeps).indexBy(c => c.id);
    }

    private static fetchByType(): Map<string, Array<Creep>>
    {
        return Creeps._my.value.data.groupBy(c => c.type);
    }

    private static my(): Array<Creep>
    {
        return Creeps._my.value.data;
    }

    private static ofType(type: string): Array<Creep>
    {
        return Creeps._byType.value.get(Names.type(type)) || new Array();
    }

    static setup()
    {
        Objects.setGetter(Creep.prototype, "type", Creeps.type);
        Objects.setGetter(Creep.prototype, "workParts", Creeps.workParts);

        Objects.setGetter(Creep, "my", Creeps.my);
        Objects.setFunction(Creep, "ofType", Creeps.ofType);
    }
}