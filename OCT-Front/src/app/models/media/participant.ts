import { Nullable } from "src/app/services/infrastructure/common";
import { CountryModel } from "../infrastructure/country";

export interface ParticipantModel {
    id: string;
    name: string;
    surname: string;
    birthday: Nullable<string>;
    description: string;
    role: ParticipantRole;
    country: string;
}

export interface ParticipantViewModel {
    id: string;
    name: string;
    surname: string;
    birthday: Nullable<string>;
    description: string;
    role: ParticipantRole;
    country: Nullable<CountryModel>;
}

export enum ParticipantRole {
    Actor,
    Producer
}

export interface ParticipantFilters {
    nameStartsWith: Nullable<string>;
    surnameStartsWith: Nullable<string>;
    role: Nullable<ParticipantRole>;
    limit: Nullable<number>;
    offset: Nullable<number>;
}

export interface UpdateParticipantModel {
    id: string;
    name: string;
    surname: string;
    birthday: Nullable<string>;
    description: string;
    role: ParticipantRole;
    country: string;
}