import * as _ from "lodash";

export class Constructions
{
    private static _my: ConstructionSite[] = [];
    private static _notWalls: ConstructionSite[];

    static get my(): ConstructionSite[] { return Constructions._my; }
    static get notWalls(): ConstructionSite[] { return Constructions._notWalls; }

    static initialize()
    {
        Constructions._my = _.values(Game.constructionSites);
        Constructions._notWalls = Constructions._my.filter(s => s.structureType != STRUCTURE_WALL);
    }
}