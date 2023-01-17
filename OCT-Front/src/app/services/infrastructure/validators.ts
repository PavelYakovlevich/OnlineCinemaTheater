import { NodeWithI18n } from '@angular/compiler';
import { AbstractControl, ValidationErrors, ValidatorFn, Validators as NgValidators } from '@angular/forms';
import { DateAdapter } from '@angular/material/core';

export class Validators {
    private static readonly hasOneUppercaseLetterRegex = new RegExp('[A-Z]');
    private static readonly hasOneLowercaseLetterRegex = new RegExp('[a-z]');
    private static readonly doesntHaveSpecialCharactersLetterRegex = new RegExp('^[\\w]+$');

    static allCharacters(control: AbstractControl<any, any>): ValidationErrors | null { 
        const value: string = control.value ?? ''

        return !/\\d+/.test(value) ? null : { 'numbers_were_found': 'Numbers were found' }
    }

    static email(control: AbstractControl<any, any>): ValidationErrors | null { 
        const controlValue: string = (control.value as string).trim()

        if (controlValue.length === 0) {
            return {
                'required': `Email is required`
            }
        }

        const result = NgValidators.email(control)

        if (result && Object.keys(result).find(k => k === 'email')) return {
            'email': 'Invalid email'
        }

        return null
    }

    static mediaName(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.validateCore(control, [NgValidators.required, Validators.notWhitespace, NgValidators.maxLength(50)])
    }

    static mediaBudget(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.positiveNullableNumber(control)
    }

    static mediaAid(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.positiveNullableNumber(control)
    }

    static password(minLength: number, maxLength: number): ValidatorFn { 
        return (control: AbstractControl<any, any>) => Validators.passwordCore(control, minLength, maxLength)
    }

    static notWhitespace(control: AbstractControl<any, any>): ValidationErrors | null {
        const isWhitespace = (control.value || '').trim().length === 0;
        const isValid = !isWhitespace;
        return isValid ? null : { 'whitespace': true };
    }

    static personName(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.validateCore(control, [NgValidators.required, Validators.notWhitespace, NgValidators.maxLength(50)])
    }


    static participantName(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.validateCore(control, [NgValidators.required, Validators.notWhitespace, NgValidators.maxLength(50)])
    }

    static participantSurname(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.validateCore(control, [NgValidators.required, Validators.notWhitespace, NgValidators.maxLength(50)])
    }

    static participantBirthday(control: AbstractControl<any, any>): ValidationErrors | null {
        return Validators.personBirthday(control)
    }

    static personBirthday(control: AbstractControl<any, any>): ValidationErrors | null {
        if (NgValidators.required(control) !== null) return { 'required': true }

        const value = new Date(control.value ?? '')

        const utcNow = new Date(new Date().toUTCString())

        if (value > utcNow) {
            return {
                'future_date': 'Birthday must be in past'
            }
        }

        return null;
    }

    private static validateCore(control: AbstractControl<any, any>, validators: ValidatorFn[]): ValidationErrors | null {
        for(let validator of validators) {
            const validationResult = validator(control)
            if (validationResult) return validationResult
        }
        
        return null
    }

    private static passwordCore(control: AbstractControl<any, any>, minLength: number, maxLength: number): ValidationErrors | null {
        const controlValue: string = control.value;

        if (controlValue.length < minLength || controlValue.length > maxLength) {
            return {
                'length': `Password length must be less than ${minLength} and greater than ${maxLength}`
            }
        }

        if (!Validators.hasOneLowercaseLetterRegex.test(controlValue)) {
            return {
                'at_least_one_lowercase_letter': 'Password must contain at least one lowercase letter'
            }
        }

        if (!Validators.hasOneUppercaseLetterRegex.test(controlValue)) {
            return {
                'at_least_one_uppercase_letter': 'Password must contain at least one uppercase letter'
            }
        }

        if (!Validators.doesntHaveSpecialCharactersLetterRegex.test(controlValue)) {
            return {
                'invalid_characters': 'Password can contain only letters, digits and \'_\' character'
            }
        }

        return null;
    }  

    private static positiveNullableNumber(control: AbstractControl): ValidationErrors | null {
        if (!control.value) return null

        const value = Number(control.value)

        if (Number.isNaN(value)) return { 'not_a_number': 'Value must be a number' }
        if (value < 0) return {'negative_number': 'Value is less than 0'}

        return {};
    }
}