import React from 'react';
import styles from './Input.module.css';

interface InputProps {
    label: string;
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    type?: 'text' | 'password';
    placeholder?: string;
}

export const Input: React.FC<InputProps> = ({
    label,
    value,
    onChange,
    type = 'text',
    placeholder
}) => {
    return (
        <div className={styles.inputWrapper}>
            <label className={styles.label}>{label}</label>
            <input
                className={styles.input}
                type={type}
                value={value}
                onChange={onChange}
                placeholder={placeholder}
            />
        </div>
    );
};