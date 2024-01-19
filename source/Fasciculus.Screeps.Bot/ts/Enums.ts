
export enum CreepType
{
    Weller = "W",
    Supplier = "S",
    Upgrader = "U",
    Builder = "B"
}

export enum CreepState
{
    Idle = "idle",
    Suicide = "suicide",
    ToContainer = "toContainer",
    ToController = "toController",
    ToCustomer = "toCustomer",
    ToSite = "toSite",
    ToSource = "toSource",
    ToSupply = "toSupply",
    Harvest = "harvest",
    Withdraw = "withdraw",
    Transfer = "transfer",
    Upgrade = "upgrade",
    Build = "build"
}

export const CreepStateText: string[] =
    [
        "I",
        "Sc",
        ">Ct",
        ">Cl",
        ">Cu",
        ">Si",
        ">W",
        ">Su",
        "H",
        "W",
        "T",
        "U",
        "B"
    ];
