import { Nullable } from "src/app/services/infrastructure/common";

export interface UserModel {
    id: string;
    name: Nullable<string>;
    surname: Nullable<string>;
    middleName: Nullable<string>;
    birthday: Nullable<string>;
    description: Nullable<string>;
}