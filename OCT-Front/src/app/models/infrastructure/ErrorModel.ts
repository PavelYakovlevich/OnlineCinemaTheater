export class ErrorModel {
    constructor(
        public status: number,
        public message?: string,
        public error?: ErrorDetails,
    ) {}
}

export interface ErrorDetails {
    Detail: string,
    Instance: string,
    Status: number,
    Title: string,
    Type: any
}