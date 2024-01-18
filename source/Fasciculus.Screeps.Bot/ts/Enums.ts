
export enum CreepType
{
    Weller = "W",
    Supplier = "S",
    Upgrader = "U",
    Builder = "B"
}

export enum CreepState
{
    Idle,
    Suicide,
    MoveToContainer,
    MoveToController,
    MoveToCustomer,
    MoveToSite,
    MoveToSource,
    MoveToSupply,
    Harvest,
    Withdraw,
    Transfer,
    Upgrade,
    Build
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
