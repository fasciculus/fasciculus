import * as _ from "lodash";

import { Bodies, BodyTemplate } from "./Bodies";
import { CreepBase, Creeps } from "./Creeps";
import { CreepState, CreepType } from "./Enums";
import { Extensions } from "./Extensions";
import { GameWrap } from "./GameWrap";
import { SupplierMemory } from "./Memories";
import { Spawns } from "./Spawns";
import { Customer, IdCustomer, IdSupply, Supply } from "./Types";
import { Utils } from "./Utils";
import { Stores } from "./Stores";
import { Statistics } from "./Statistics";

const MIN_SUPPLY_ENERGY = 10;

const SUPPLIER_TEMPLATE: BodyTemplate =
{
    chunks:
        [
            { cost: 200, parts: [CARRY, MOVE, CARRY, MOVE] },
            { cost: 200, parts: [CARRY, MOVE, CARRY, MOVE] },
            { cost: 100, parts: [CARRY, MOVE] },
            { cost: 100, parts: [CARRY, MOVE] },
            { cost: 100, parts: [CARRY, MOVE] },
        ]
};

const SUPPLIER_MOVE_TO_OPTS: MoveToOpts =
{
    visualizePathStyle:
    {
        stroke: "#ff0"
    }
};

export class Supplier extends CreepBase
{
    get memory(): SupplierMemory { return super.memory as SupplierMemory; }

    get supply(): Supply | undefined { return GameWrap.get(this.memory.supply); }
    set supply(value: Supply | undefined) { this.memory.supply = value?.id; }

    get customer(): Customer | undefined { return GameWrap.get(this.memory.customer); }
    set customer(value: Customer | undefined) { this.memory.customer = value?.id; }

    constructor(creep: Creep)
    {
        super(creep);
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.executeIdle(); break;
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    private executeIdle()
    {
        Statistics.addSupplied(this.energy / 25);
    }

    private executeToSupply()
    {
        let supply = this.supply;

        if (!supply) return;

        this.moveTo(supply, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeToCustomer()
    {
        let customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, SUPPLIER_MOVE_TO_OPTS);
    }

    private executeWithdraw()
    {
        let supply = this.supply;

        if (!supply) return;

        this.withdraw(supply, RESOURCE_ENERGY);
    }

    private executeTransfer()
    {
        let customer = this.customer;

        if (!customer) return;

        let amount = Math.min(this.energy, Stores.freeEnergyCapacity(customer));

        this.transfer(customer, RESOURCE_ENERGY, amount);
        Statistics.addSupplied(amount);

        if (customer instanceof Creep)
        {
            this.customer = undefined;
            this.state = CreepState.Idle;
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

        if (supply && !Supplier.hasEnergy(supply))
        {
            this.supply = supply = undefined;
        }

        if (supply)
        {
            return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
        }

        return CreepState.Idle;
    }

    private prepareMoveToSupply(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!Supplier.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    private prepareMoveToCustomer(): CreepState
    {
        let customer = this.customer;

        if (!customer) return this.prepareIdle();
        if (this.energy == 0) return this.prepareIdle();
        if (!Supplier.hasCapacity(customer)) return this.prepareIdle();

        return this.inRangeTo(customer) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private prepareWithdraw(): CreepState
    {
        let supply = this.supply;

        if (!supply) return this.prepareIdle();
        if (this.freeEnergyCapacity == 0) return this.prepareIdle();
        if (!Supplier.hasEnergy(supply)) return this.prepareIdle();

        return this.inRangeTo(supply) ? CreepState.Withdraw : CreepState.ToSupply;
    }

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

    static hasEnergy(supply: Supply): boolean
    {
        return Stores.energy(supply) >= MIN_SUPPLY_ENERGY;
    }

    static hasCapacity(customer: Customer): boolean
    {
        let minCapacity = customer instanceof Creep ? 20 : 0;

        return Stores.freeEnergyCapacity(customer) > minCapacity;
    }
}

interface SupplierInfo
{
    supplier: Supplier;
    name: string;
    pos: RoomPosition;
    energy: number;
    capacity: number;
    supply: Supply | undefined;
    customer: Customer | undefined;
}

interface SupplyInfo
{
    supply: Supply;
    id: IdSupply;
    energy: number;
    capacity: number;
}

interface CustomerInfo
{
    customer: Customer;
    id: IdCustomer;
    priority: number;
    demand: number;
}

export class Suppliers
{
    private static _all: Supplier[] = [];

    static get all(): Supplier[] { return Suppliers._all; }

    static get supplied(): number { return _.sum(Suppliers._all.map(s => s.energyCapacity)) / 25; }

    static initialize()
    {
        Suppliers._all = Creeps.ofType(CreepType.Supplier).map(c => new Supplier(c));

        Bodies.register(CreepType.Supplier, SUPPLIER_TEMPLATE);
    }

    static run()
    {
        Suppliers._all.forEach(s => s.prepare());

        let assigned = Suppliers.assign();

        assigned.forEach(s => s.prepare());
        Suppliers._all.forEach(s => s.execute());
    }

    private static assign(): Supplier[]
    {
        var result: Supplier[] = [];
        var suppliers: _.Dictionary<SupplierInfo> = Suppliers.findUnassignedSuppliers();

        if (Utils.isEmpty(suppliers)) return result;

        result = Suppliers.assignCustomers(suppliers);

        if (Utils.isEmpty(suppliers)) return result;

        return result.concat(Suppliers.assignSupplies(suppliers));
    }

    private static assignCustomers(unassigned: _.Dictionary<SupplierInfo>): Supplier[]
    {
        let result: Supplier[] = [];
        let suppliers: _.Dictionary<SupplierInfo> = Suppliers.findSuppliersWithEnergy(unassigned);

        if (Utils.isEmpty(suppliers)) return result;

        let customers: _.Dictionary<CustomerInfo> = Suppliers.findCustomers();

        if (Utils.isEmpty(customers)) return result;

        let sorted: CustomerInfo[] = Suppliers.sortCustomers(customers);

        for (let customerInfo of sorted)
        {
            while (customerInfo.demand > 0)
            {
                let customer: Customer = customerInfo.customer;
                let supplierInfo: SupplierInfo | undefined = Suppliers.findNearest(customer, suppliers);

                if (!supplierInfo) break;

                let supplier: Supplier = supplierInfo.supplier;

                supplier.customer = customer;
                customerInfo.demand -= supplierInfo.energy;

                delete suppliers[supplierInfo.name];
                delete unassigned[supplierInfo.name];

                result.push(supplier);
            }

            if (Utils.isEmpty(suppliers)) break;
        }

        return result;
    }

    private static assignSupplies(suppliers: _.Dictionary<SupplierInfo>): Supplier[]
    {
        let result: Supplier[] = [];
        let supplies: _.Dictionary<SupplyInfo> = Suppliers.findSupplies();
        let sorted: SupplyInfo[] = Suppliers.sortSupplies(supplies);

        for (let supplyInfo of sorted)
        {
            while (supplyInfo.energy > MIN_SUPPLY_ENERGY)
            {
                let supply: Supply = supplyInfo.supply;
                let supplierInfo: SupplierInfo | undefined = Suppliers.findNearest(supply, suppliers);

                if (!supplierInfo) break;

                let supplier: Supplier = supplierInfo.supplier;

                supplier.supply = supply;
                supplyInfo.energy -= supplierInfo.capacity;
                delete suppliers[supplierInfo.name];
                result.push(supplier);
            }

            if (Utils.isEmpty(suppliers)) break;
        }

        return result;
    }

    private static findSuppliersWithEnergy(unassigned: _.Dictionary<SupplierInfo>): _.Dictionary<SupplierInfo>
    {
        var suppliers: SupplierInfo[] = _.values(unassigned);

        suppliers = suppliers.filter(s => s.energy > 0);

        return _.indexBy(suppliers, s => s.name);
    }

    private static findCustomers(): _.Dictionary<CustomerInfo>
    {
        let spawns: Customer[] = Spawns.my.map(s => s.spawn);
        let extensions: Customer[] = Extensions.my;
        let upgraders: Customer[] = Creeps.ofType(CreepType.Upgrader);
        let builders: Customer[] = Creeps.ofType(CreepType.Builder);
        let repairers: Customer[] = Creeps.ofType(CreepType.Repairer);
        var customers: Customer[] = spawns.concat(extensions).concat(upgraders).concat(builders).concat(repairers);

        customers = customers.filter(c => Supplier.hasCapacity(c));

        let infos: CustomerInfo[] = customers.map(Suppliers.createCustomerInfo);
        let result: _.Dictionary<CustomerInfo> = _.indexBy(infos, i => i.customer.id);

        Suppliers.adjustCustomerDemands(result);
        Suppliers.filterCustomers(result);

        return result;
    }

    private static findSupplies(): _.Dictionary<SupplyInfo>
    {
        let wellers: Supply[] = Creeps.ofType(CreepType.Weller);
        let supplies: Supply[] = wellers;

        supplies = supplies.filter(s => Supplier.hasEnergy(s));

        let infos: SupplyInfo[] = supplies.map(Suppliers.createSupplyInfo);
        let result: _.Dictionary<SupplyInfo> = _.indexBy(infos, i => i.supply.id);

        Suppliers.adjustSupplyEnergies(result);
        Suppliers.filterSupplies(result);

        return result;
    }

    private static adjustCustomerDemands(infos: _.Dictionary<CustomerInfo>)
    {
        for (let supplier of Suppliers._all)
        {
            let customer = supplier.customer;

            if (!customer) continue;

            let info = infos[customer.id];

            if (!info) continue;

            info.demand -= supplier.energy;
        }
    }

    private static adjustSupplyEnergies(infos: _.Dictionary<SupplyInfo>)
    {
        for (let supplier of Suppliers._all)
        {
            let supply = supplier.supply;

            if (!supply) continue;

            let info = infos[supply.id];

            if (!info) continue;

            info.energy -= supplier.freeEnergyCapacity;
        }
    }

    private static filterCustomers(infos: _.Dictionary<CustomerInfo>)
    {
        let serveds = _.values<CustomerInfo>(infos).filter(i => i.demand <= 0);

        for (let served of serveds)
        {
            delete infos[served.id];
        }
    }

    private static filterSupplies(infos: _.Dictionary<SupplyInfo>)
    {
        let useds = _.values<SupplyInfo>(infos).filter(i => i.energy <= 0);

        for (let used of useds)
        {
            delete infos[used.id];
        }
    }

    private static findUnassignedSuppliers(): _.Dictionary<SupplierInfo>
    {
        var infos: SupplierInfo[] = Suppliers._all.map(Suppliers.createSupplierInfo);

        infos = infos.filter(i => !i.supply && !i.customer);

        return _.indexBy(infos, i => i.name);
    }

    private static createSupplierInfo(supplier: Supplier): SupplierInfo
    {
        let info: SupplierInfo =
        {
            supplier: supplier,
            name: supplier.name,
            pos: supplier.pos,
            energy: supplier.energy,
            capacity: supplier.freeEnergyCapacity,
            supply: supplier.supply,
            customer: supplier.customer
        }

        return info;
    }

    private static createSupplyInfo(supply: Supply): SupplyInfo
    {
        let info: SupplyInfo =
        {
            supply: supply,
            id: supply.id,
            energy: Stores.energy(supply),
            capacity:Stores.freeEnergyCapacity(supply)
        };

        return info;
    }

    private static createCustomerInfo(customer: Customer): CustomerInfo
    {
        let info: CustomerInfo =
        {
            customer: customer,
            id: customer.id,
            priority: Suppliers.customerPriority(customer),
            demand: Stores.freeEnergyCapacity(customer)
        }

        return info;
    }

    private static customerPriority(customer: Customer): number
    {
        if (customer instanceof StructureSpawn) return 1;
        if (customer instanceof StructureExtension) return 2;

        return 3;
    }

    private static sortSupplies(supplies: _.Dictionary<SupplyInfo>): SupplyInfo[]
    {
        var result: SupplyInfo[] = _.values(supplies);

        return result.sort(Suppliers.compareSupplies);
    }

    private static sortCustomers(customers: _.Dictionary<CustomerInfo>): CustomerInfo[]
    {
        var result: CustomerInfo[] = _.values(customers);

        return result.sort(Suppliers.compareCustomers);
    }

    private static compareCustomers(a: CustomerInfo, b: CustomerInfo): number
    {
        let result = a.priority - b.priority;

        if (result != 0) return result;

        return b.demand - a.demand;
    }

    private static compareSupplies(a: SupplyInfo, b: SupplyInfo): number
    {
        let result = a.capacity - b.capacity;

        if (result != 0) return result;

        return b.energy - a.energy;
    }

    private static findNearest(target: Customer | Supply, suppliers: _.Dictionary<SupplierInfo>): SupplierInfo | undefined
    {
        return target.pos.findClosestByPath(_.values<SupplierInfo>(suppliers)) || undefined;
    }
}