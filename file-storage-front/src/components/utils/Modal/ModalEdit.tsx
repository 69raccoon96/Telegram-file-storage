import React, {useState} from 'react';
import "./Modal.scss"
import {Button} from "../Button/Button";
import Modal from "./Modal";
import {useDispatch} from "react-redux";
import {filesSlice} from "../../../redux/filesSlice";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";
import {fetchEditFileName} from "../../../redux/fileThunks";

const {closeModal} = filesSlice.actions;

export const ModalEdit: React.FC<{ id: string }> = ({id}) => {
    const dispatch = useDispatch();
    const file = useAppSelector((state) => state.filesReducer.files.find((file) => file.fileId === id));
    const [value, changeValue] = useState(file?.fileName);
    return (
        <Modal>
            <div className={"modal-confirm"}>
                <h2 className={"modal-confirm__h2"}>Переименовать</h2>
                <p className={"modal-confirm__p"} style={{width: "100%"}}><input value={value}
                                                                                 onChange={(e) => changeValue(e.target.value)}
                                                                                 style={{width: "100%"}} type={"text"}/>
                </p>
                <div className={"modal-confirm__btns"}>
                    <Button onClick={() => {dispatch(fetchEditFileName({id, fileName: value || ""}))
                    }}>Да</Button>
                    <Button onClick={() => dispatch(closeModal())} type={"transparent"}>Нет</Button>
                </div>
            </div>
        </Modal>
    )
}

