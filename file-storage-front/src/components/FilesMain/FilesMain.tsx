import React, {useEffect} from 'react';
import "./FilesMain.scss"
import Paginator from '../utils/Paginator/Paginator';
import FragmentFile from "./FragmentFile";
import {useHistory} from "react-router-dom";
import * as queryString from "querystring";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {configFilters} from "./ConfigFilters";
import {SubmitHandler, useForm} from "react-hook-form";
import {Select} from "../utils/Inputs/Select";
import {fetchFiles, fetchFilters} from "../../redux/mainThunks";
import {SelectTime} from "../utils/Inputs/SelectDate";
import {Button} from '../utils/Button/Button';

const FilesMain = () => {
    const filesReducer = useAppSelector((state) => state.filesReducer);
    const filesData = filesReducer.files;
    const filesNames = filesReducer.filesNames;
    const paginator = useAppSelector((state) => state.filesReducer.paginator)
    const chats = filesReducer.chats;
    const senders = filesReducer.senders;
    const dispatch = useAppDispatch();
    const history = useHistory();

    useEffect(() => {
        dispatch(fetchFilters());
        const {fileName, chats, senderId, categories, date} = GetQueryParamsFromUrl(history);
        setValue("fileName", fileName);
        setValue("sendersIds", senderId);
        setValue("categories", categories);
        setValue("chatIds", chats);
        setValue("date", date);
    }, []);

    const {optionsName, optionsSender, optionsChat, optionsCategory} = configFilters(filesNames, chats, senders);

    const {register, handleSubmit, formState: {errors}, setValue, getValues} = useForm();
    const dispatchValuesForm: SubmitHandler<any> = (formData) => {
        AddToUrlQueryParams(history, formData);
        dispatch(fetchFiles({skip: 0, take: 5, ...formData}));
    };

    const FragmentsFiles = filesData.map((f) => <FragmentFile key={f.fileId} file={f}/>);

    const onChangeForm = handleSubmit(dispatchValuesForm);
    const setValueForm = (name: any, value: any) => {
        setValue(name, value, {
            shouldValidate: true,
            shouldDirty: true
        });
    }
    return (
        <div className={"files-main"}>
            <h2 className={"files-main__title"}>Файлы</h2>
            <div className={"files-main__content"}>
                <form className={"files"} onSubmit={onChangeForm}>
                    <h3 className={"files__title"}>Название</h3>
                    <h3 className={"files__title"}>Дата</h3>
                    <h3 className={"files__title"}>Формат</h3>
                    <h3 className={"files__title"}>Отправитель</h3>
                    <h3 className={"files__title"}>Чаты</h3>
                    <Select name={"fileName"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValueForm}
                            values={getValues("fileName")} options={optionsName} isMulti={false}/>
                    <SelectTime name={"date"} className={"files__filter files__filter_select"} register={register}
                                onChangeForm={onChangeForm} setValue={setValueForm}
                                values={getValues("date")} placeholder={"Выберите дату"}/>
                    <Select name={"categories"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValueForm}
                            values={getValues("categories")} options={optionsCategory} isMulti={true}/>
                    <Select name={"sendersIds"} className={"files__filter files__filter_select"} register={register}
                            onChangeForm={onChangeForm} setValue={setValueForm}
                            values={getValues("sendersIds")} options={optionsSender} isMulti={true}/>
                    <Select name={"chatIds"} className={"files__filter files__filter_last files__filter_select"}
                            register={register}
                            onChangeForm={onChangeForm} setValue={setValueForm}
                            values={getValues("chatIds")} options={optionsChat} isMulti={true}/>
                    <div style={{gridColumn: "1/5", borderBottom: "1px #E9E9E9 solid"}} className={"files__filter"}/>
                    <div style={{gridColumn: "5/6", borderBottom: "1px #E9E9E9 solid", display:"flex"}} className={"files__filter"}>
                        <Button type={"danger"} style={{flex:"1 1 auto", marginRight:"10px"}}>
                            Запросить файлы &#128269;
                        </Button>
                    </div>
                    {FragmentsFiles}
                </form>
            </div>
            <Paginator paginator={paginator}/>
        </div>
    );
};


//#region utils
const AddToUrlQueryParams = (history: any, values: Object) => {
    const urlParams = {};
    Object.keys(values).forEach(key => {
        // @ts-ignore
        const value = values[key];
        if (value) {
            if (value instanceof Array) {
                if (value.length > 0) {
                    // @ts-ignore
                    urlParams[key] = value.join(`&`)
                }
            } else {
                // @ts-ignore
                urlParams[key] = value;
            }
        }
    })

    history.push({
        search: queryString.stringify(urlParams),
    })
};

const GetQueryParamsFromUrl = (history: any) => {
    const urlSearchParams = new URLSearchParams(history.location.search);
    const fileName = urlSearchParams.get("fileName");
    const senderId = urlSearchParams.get("sendersIds")?.split("&")?.map((e) => e);
    const categories = urlSearchParams.get("categories")?.split("&") as any;
    const date = urlSearchParams.get("date");
    const chats = urlSearchParams.get("chatIds")?.split("&")?.map((e) => e);
    return {fileName, senderId, categories, date, chats};
}
//#endregion

export default FilesMain;
