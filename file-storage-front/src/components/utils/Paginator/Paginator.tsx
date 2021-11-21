import React, {useState} from 'react';
import "./Paginator.scss"

const Paginator = ({count}: { count: number }) => {
    const [currentPage, changePage] = useState(1);
    count = 40;
    let pages: Array<number> = [];
    const dif = 3;
    let countPages = Math.ceil(count / 5);
    for (let i = Math.max(currentPage - dif, 2); i <= Math.min(currentPage + dif, countPages - 1); i++) {
        pages.push(i);
    }

    return (
        <div className={"paginator"}>
            <button className={"paginator__item"} disabled={currentPage === 1}
                    onClick={() => changePage(currentPage - 1)}>←
            </button>
            <div className={"paginator__item " + (1 === currentPage ? "paginator__item_active" : "")}
                 onClick={() => changePage(1)}>1
            </div>
            {(currentPage > 2 + dif && countPages > 10) && <div className={"paginator__nothing"}>...</div>}
            {pages.map((e) => <div className={"paginator__item " + (e === currentPage ? "paginator__item_active" : "")}
                                   onClick={() => changePage(e)} key={e}>{e}</div>)}
            {countPages > 1 &&
            <>{(currentPage < countPages - dif - 1) && <div className={"paginator__nothing"}>...</div>}
                <div onClick={() => changePage(countPages)}
                     className={"paginator__item " + (countPages === currentPage ? "paginator__item_active" : "")}>{countPages}</div>
                <button className={"paginator__item"} disabled={currentPage === countPages}
                        onClick={() => changePage(currentPage + 1)}>→
                </button>
            </>}
        </div>
    )
}

export default Paginator;