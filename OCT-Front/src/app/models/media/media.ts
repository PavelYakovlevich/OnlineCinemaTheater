import { AgeRating } from "./age-rating";
import { ParticipantModel } from "./participant";
import { GenreModel } from "./genre";
import { Nullable } from "src/app/services/infrastructure/common";
import { CountryModel } from "../infrastructure/country";

export interface MediaModel {
    id: string;
    name: string;
    budget: Nullable<number>;
    aid: Nullable<number>;
    description: string;
    issueDate: string;
    isFree: boolean;
    ageRating: AgeRating;
    isTvSerias: boolean;
    isVisible: boolean;
    country: string;
    participants: ParticipantModel[];
    genres: GenreModel[];
}

export interface MediaFiltersModel {
    country: Nullable<string>;
    genres: string[];
    nameStartsWith: Nullable<string>;
    isTvSerias: Nullable<boolean>;
}


export interface MediaViewModel {
    id: string;
    name: string;
    budget: Nullable<number>;
    aid: Nullable<number>;
    description: string;
    issueDate: string;
    isFree: boolean;
    ageRating: AgeRating;
    isTvSerias: boolean;
    isVisible: boolean;
    country: Nullable<CountryModel>;
    participants: ParticipantModel[];
    genres: GenreModel[];
}
