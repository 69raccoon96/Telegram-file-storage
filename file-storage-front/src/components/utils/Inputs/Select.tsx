import React, {useState} from 'react';
import {Category} from "../../../models/File";
import "./Select.scss";
import {OutsideAlerter} from "../OutSideAlerter/OutSideAlerter";


type Props = {
    name: string, onChangeForm: any,
    className?: string, register: any, getValues: any, setValue: any, options: Array<{ value: any, label: string }>,
    placeholder?: string, isMulti?: boolean
}

export const Select: React.FC<Props> = ({
                                             name,
                                             onChangeForm,
                                             className,
                                             register,
                                             getValues,
                                             setValue,
                                             options,
                                             placeholder,
                                             isMulti
                                         }) => {
    if (!placeholder)
        placeholder = "Введите текст";
    const [isOpen, changeOpen] = useState(false);
    const [text, changeText] = useState("");
    const regexp = new RegExp(`${text}`, 'i');
    const ui = options.filter((elem) => !!elem.label.match(regexp))
        .map((elem) => {
            const onChange = () => {
                if (isMulti) {
                    const values = getValues(name);
                    if (values?.includes(elem.value)) {
                        setValue(name, values.filter((v: Category) => v !== elem.value));
                    } else if (values) {
                        setValue(name, [...values, elem.value])
                    } else {
                        setValue(name, [elem.value])
                    }
                } else {
                    setValue(name, elem.value)
                }

                onChangeForm();
            }

            const values = getValues(name);
            return <li
                className={"select__option " + (values && (values.includes(elem.value)) ? "select__option_active" : "")}
                onClick={onChange}>{elem.label}</li>;
        })

    return (
        <OutsideAlerter className={className} onOutsideClick={() => {
            changeOpen(false);
        }}>
            <div className={"select"}>
                <input className={"select__field"} onClick={() => changeOpen(!isOpen)} value={text} onChange={(e) => {
                    changeText(e.target.value)
                }} placeholder={placeholder}/>
                <ul className={"select__list " + (isOpen ? "select__list_open" : "")} onBlur={() => changeOpen(false)}>
                    <div className="select__scroll">
                        {ui}
                    </div>
                </ul>
                <select multiple={true} {...register(name, {onChange: () => onChangeForm})}/>
            </div>
        </OutsideAlerter>
    )
}

