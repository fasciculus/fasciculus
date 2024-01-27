import { Bodies, BodyTemplate } from "./Bodies";
import { Builders } from "./Builders";
import { Dictionaries } from "./Collections";
import { CreepState, CreepType, Customer, CustomerId, Dictionary, Supply, SupplyId, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Extensions } from "./Extensions";
import { GameWrap } from "./GameWrap";
import { Positions } from "./Positions";
import { profile } from "./Profiling";
import { Repairers } from "./Repairers";
import { Spawns } from "./Spawns";
import { Stores } from "./Stores";
import { Upgraders } from "./Upgraders";
import { Wellers } from "./Wellers";
import { SUPPLIER_ENOUGH_ENERGY_RATIO, SUPPLIER_MIN_CREEP_FREE_RATIO, SUPPLIER_MIN_SUPPLY_ENERGY, SUPPLIER_PERFORMANCE_FACTOR } from "./_Config";

const SUPPLIER_TEMPLATE: BodyTemplate = BodyTemplate.create([CARRY, MOVE, CARRY, MOVE]).add([CARRY, MOVE, CARRY, MOVE]).add([CARRY, MOVE], 21);

const SUPPLIER_MOVE_TO_OPTS: MoveToOpts =
{
    reusePath: 6,
    ignoreCreeps: false,

    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

class SupplierSupport
{
    static hasEnergy<M extends CreepBaseMemory>(supply: Supply | CreepBase<M>): boolean
    {
        let energy = supply instanceof CreepBase ? supply.energy : Stores.energy(supply);

        return energy >= SUPPLIER_MIN_SUPPLY_ENERGY;
    }

    static hasCapacity(customer: Customer): boolean
    {
        let maxRatio = customer instanceof Creep ? SUPPLIER_MIN_CREEP_FREE_RATIO : 1;

        return Stores.energyRatio(customer) < maxRatio;
    }
}

interface SupplierMemory extends CreepBaseMemory
{
    customer?: CustomerId;
    supply?: SupplyId;
    supplying?: boolean;

    travelled?: number;
    handled?: number;
}

export class Supplier extends CreepBase<SupplierMemory>
{
    private _supply?: Supply;
    private _customer?: Customer;

    private _travelled: number;
    private _handled: number;

    private _supplying: boolean = false;

    get supply(): Supply | undefined { return this._supply; }

    set supply(value: Supply | undefined)
    {
        this._supply = value;
        this.memory.supply = value?.id;

        if (value && this._supplying)
        {
            this.supplying = false;
        }
    }

    get customer(): Customer | undefined { return this._customer; }

    set customer(value: Customer | undefined)
    {
        this._customer = value;
        this.memory.customer = value?.id;

        if (value && !this._supplying)
        {
            this.supplying = true;
        }
    }

    get supplying(): boolean { return this._supplying; }
    private set supplying(value: boolean) { this._supplying = this.memory.supplying = value; }

    get hasEnoughEnergy(): boolean { return this.energyRatio > SUPPLIER_ENOUGH_ENERGY_RATIO; }

    get travelled(): number { return this._travelled; }
    private incrementTravelled() { this._travelled = this.memory.travelled = this._travelled + 1; }

    get handled(): number { return this._handled; }
    private addHandled(amount: number) { this._handled = this.memory.handled = this._handled + amount; }

    get performance(): number { return (this.handled / this.travelled) * SUPPLIER_PERFORMANCE_FACTOR; }

    constructor(creep: Creep)
    {
        super(creep, CreepType.Supplier);

        let memory = this.memory;

        this.supply = GameWrap.get(memory.supply);
        this._customer = GameWrap.get(memory.customer);

        this._travelled = memory.travelled || 1;
        this._handled = memory.handled || this.energyCapacity;

        this.supplying = (memory.supplying || false) && this.energy > 0;
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    private executeToSupply()
    {
        this.incrementTravelled();

        let supply = this.supply;

        if (!supply) return;

        this.moveTo(supply, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeToCustomer()
    {
        this.incrementTravelled();

        let customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, SUPPLIER_MOVE_TO_OPTS);
    }

    @profile
    private executeWithdraw()
    {
        let supply = this.supply;

        if (!supply) return;

        let amount = Math.min(this.freeEnergyCapacity, Stores.energy(supply));

        this.withdraw(supply, RESOURCE_ENERGY, amount);
        this.onHandled(amount);
    }

    @profile
    private executeTransfer()
    {
        let customer = this.customer;

        if (!customer) return;

        let amount: number = Math.min(this.energy, Stores.freeEnergyCapacity(customer));

        this.transfer(customer, RESOURCE_ENERGY, amount);

        if (customer instanceof Creep) this.customer = undefined;

        this.onHandled(amount);
    }

    private onHandled(amount: number)
    {
        this.addHandled(amount);

        if (this._travelled > 100)
        {
            let memory = this.memory;

            this._travelled = memory.travelled = this._travelled / 2;
            this._handled = memory.handled = this._handled / 2;
        }
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle();
            case CreepState.ToSupply: this.state = this.prepareMoveToSupply();
            case CreepState.ToCustomer: this.state =  this.prepareMoveToCustomer();
            case CreepState.Withdraw: this.state =  this.prepareWithdraw();
            case CreepState.Transfer: this.state = this.prepareTransfer();
        }
    }

    @profile
    private prepareIdle(): CreepState
    {
        let supply: Supply | undefined = this.supply;
        let customer: Customer | undefined = this.customer;

        if (customer && !Supplier.hasCapacity(customer))
        {
            this.customer = customer = undefined;
        }

        if (customer && this.energy == 0)
        {
            this.customer = customer = undefined;
        }

        if (customer)
        {
            return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
        }

        if (supply && this.freeEnergyCapacity == 0)
        {
            this.supply = supply = undefined;
        }

        if (supply && !SupplierSupport.hasEnergy(supply))
        {
            this.supply = supply = undefined;
        }

        if (supply)
        {
            return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
        }

        return CreepState.Idle;
    }

    @profile
    private prepareMoveToSupply(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!SupplierSupport.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    @profile
    private prepareMoveToCustomer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();
        if (this.energy == 0) return this.prepareIdle();
        if (!Supplier.hasCapacity(customer)) return this.prepareIdle();

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    @profile
    private prepareWithdraw(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!SupplierSupport.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    @profile
    private prepareTransfer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();
        if (this.energy == 0) return this.prepareIdle();
        if (!Supplier.hasCapacity(customer)) return this.prepareIdle();

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private inRangeTo(target: Supply | Customer): boolean
    {
        return this.pos.inRangeTo(target, 1);
    }

    static hasCapacity(customer: Customer): boolean
    {
        let maxRatio = customer instanceof Creep ? 0.8 : 1;

        return Stores.energyRatio(customer) < maxRatio;
    }
}

class Assignments
{
    private _earmarked: Dictionary<number> = {};
    private _supplied: Dictionary<number> = {};

    private _canSupply: Dictionary<Supplier> = {};
    private _canSupplyCount: number = 0;

    private _needsSupply: Dictionary<Supplier> = {};
    private _needsSupplyCount: number = 0;

    earmarked(id: SupplyId): number { return this._earmarked[id] || 0; }
    supplied(id: CustomerId): number { return this._supplied[id] || 0; }

    get canSupply(): boolean { return this._canSupplyCount > 0; }
    get needsSupply(): boolean { return this._needsSupplyCount > 0; }

    constructor(suppliers: Vector<Supplier>)
    {
        for (let supplier of suppliers)
        {
            if (supplier.spawning) continue;

            let supply = supplier.supply;

            if (supply)
            {
                let earmarked = this._earmarked;
                let id = supply.id;

                earmarked[id] = (earmarked[id] || 0) + supplier.freeEnergyCapacity;
                continue;
            }

            let customer = supplier.customer;

            if (customer)
            {
                let supplied = this._supplied;
                let id = customer.id;

                supplied[id] = (supplied[id] || 0) + supplier.energy;
                continue;
            }

            if (supplier.supplying)
            {
                this._canSupply[supplier.name] = supplier;
                ++this._canSupplyCount;
            }
            else
            {
                if (supplier.hasEnoughEnergy)
                {
                    this._canSupply[supplier.name] = supplier;
                    ++this._canSupplyCount;
                }
                else
                {
                    this._needsSupply[supplier.name] = supplier;
                    ++this._needsSupplyCount;
                }
            }
        }
    }

    assignSupply(supply: Supply): Supplier | undefined
    {
        const needsSupply = this._needsSupply;
        const suppliers = Dictionaries.values(needsSupply);
        const supplier = Positions.closestByRange(supply, suppliers);

        if (supplier)
        {
            supplier.supply = supply;
            delete needsSupply[supplier.name];
            --this._needsSupplyCount;
        }

        return supplier;
    }

    assignCustomer(customer: Customer): Supplier | undefined
    {
        const canSupply = this._canSupply;
        const suppliers = Dictionaries.values(canSupply);
        const supplier = Positions.closestByRange(customer, suppliers);

        if (supplier)
        {
            supplier.customer = customer;
            delete canSupply[supplier.name];
            --this._canSupplyCount;
        }

        return supplier;
    }
}

interface SupplyInfo
{
    supply: Supply;
    energy: number;
}

class Supplies
{
    private _assignments: Assignments;
    private _infos: Vector<SupplyInfo> = new Vector();

    constructor(assignments: Assignments)
    {
        this._assignments = assignments;

        this.addCreepInfos(Wellers.all.filter(SupplierSupport.hasEnergy));

        this._infos.sort(Supplies.compare);
    }

    assign(): Vector<Supplier>
    {
        const result: Vector<Supplier> = new Vector();
        const assignments: Assignments = this._assignments;

        for (let info of this._infos)
        {
            let supply = info.supply;
            let energy = info.energy;

            while (energy > 0)
            {
                let supplier = assignments.assignSupply(supply);

                if (!supplier) break;

                energy -= supplier.freeEnergyCapacity;
                result.append(supplier);

                if (!assignments.needsSupply) break;
            }

            if (!assignments.needsSupply) break;
        }

        return result;
    }

    private addCreepInfos<M extends CreepBaseMemory>(creeps: Vector<CreepBase<M>>)
    {
        const assignments: Assignments = this._assignments;
        const infos: Vector<SupplyInfo> = this._infos;

        for (let creep of creeps)
        {
            let energy = creep.energy - assignments.earmarked(creep.id);

            if (energy > 0)
            {
                infos.append({ supply: creep.creep, energy });
            }
        }
    }

    private static compare(a: SupplyInfo, b: SupplyInfo): number
    {
        return b.energy - a.energy;
    }
}

interface CustomerInfo
{
    customer: Customer;
    priority: number;
    demand: number;
}

class Customers
{
    private _assignments: Assignments;
    private _infos: Vector<CustomerInfo> = new Vector();

    constructor(assignments: Assignments)
    {
        this._assignments = assignments;

        this.addCustomerInfos(Spawns.my.map(s => s.spawn));
        this.addCustomerInfos(Extensions.my);
        this.addCreepInfos(Upgraders.all);
        this.addCreepInfos(Builders.all);
        this.addCreepInfos(Repairers.all);

        this._infos.sort(Customers.compare);
    }

    assign(): Vector<Supplier>
    {
        const result: Vector<Supplier> = new Vector();
        const assignments: Assignments = this._assignments;

        for (let info of this._infos)
        {
            let customer = info.customer;
            let demand = info.demand;

            while (demand > 0)
            {
                let supplier = assignments.assignCustomer(customer);

                if (!supplier) break;

                demand -= supplier.energy;
                result.append(supplier);

                if (!assignments.canSupply) break;
            }

            if (!assignments.canSupply) break;
        }

        return result;
    }

    private static customerPriority(customer: Customer): number
    {
        if (customer instanceof StructureSpawn) return 1;
        if (customer instanceof StructureExtension) return 2;

        return 99;
    }

    private static creepPriority<M extends CreepBaseMemory>(creep: CreepBase<M>)
    {
        switch (creep.type)
        {
            case CreepType.Repairer: return 20;
        }

        return 99;
    }

    private addCustomerInfos(customers: Vector<Customer>)
    {
        const assignments: Assignments = this._assignments;
        const infos: Vector<CustomerInfo> = this._infos;

        for (let customer of customers)
        {
            let demand = Stores.freeEnergyCapacity(customer) - assignments.supplied(customer.id);

            if (demand > 0)
            {
                let priority = Customers.customerPriority(customer);

                infos.append({ customer, priority, demand });
            }
        }
    }

    private addCreepInfos<M extends CreepBaseMemory>(creeps: Vector<CreepBase<M>>)
    {
        const assignments: Assignments = this._assignments;
        const infos: Vector<CustomerInfo> = this._infos;

        for (let creep of creeps)
        {
            if (creep.spawning) continue;

            let demand = creep.freeEnergyCapacity - assignments.supplied(creep.id);

            if (demand > 0)
            {
                let priority = Customers.creepPriority(creep);

                infos.append({ customer: creep.creep, priority, demand });
            }
        }
    }

    private static compare(a: CustomerInfo, b: CustomerInfo): number
    {
        var result = a.priority - b.priority;

        if (result == 0)
        {
            result = b.demand - a.demand;
        }

        return result;
    }
}

export class Suppliers
{
    private static _all: Vector<Supplier> = new Vector();

    static get count(): number { return Suppliers._all.length; }
    static get idleCount(): number { return Suppliers._all.filter(s => s.state == CreepState.Idle).length; }

    static get performance(): number { return Suppliers._all.sum(s => s.performance); }

    static initialize()
    {
        Suppliers._all = Creeps.ofType(CreepType.Supplier).map(c => new Supplier(c));

        Bodies.register(CreepType.Supplier, SUPPLIER_TEMPLATE);
    }

    static run()
    {
        Suppliers.prepare(Suppliers._all);
        Suppliers.prepare(Suppliers.assign());
        Suppliers.execute(Suppliers._all);
    }

    private static prepare(suppliers: Vector<Supplier>)
    {
        suppliers.forEach(s => s.prepare());
    }

    // @profile
    private static execute(suppliers: Vector<Supplier>)
    {
        suppliers.forEach(s => s.execute());
    }

    private static assign(): Vector<Supplier>
    {
        const assignments: Assignments = new Assignments(Suppliers._all);

        const assignedToSupplies = Suppliers.assignSupplies(assignments);
        const assignedToCustomers = Suppliers.assignCustomers(assignments);

        return assignedToSupplies.concat(assignedToCustomers);
    }

    @profile
    private static assignSupplies(assignments: Assignments): Vector<Supplier>
    {
        if (!assignments.needsSupply) return new Vector();

        const supplies: Supplies = new Supplies(assignments);

        return supplies.assign();
    }

    @profile
    private static assignCustomers(assignments: Assignments): Vector<Supplier>
    {
        if (!assignments.canSupply) return new Vector();

        const customers = new Customers(assignments);

        return customers.assign();
    }
}