import React from 'react';
export default function SingleDM(props: {content: Map<any, any>}){
    let backgroud = props.content.get('rg') ? 'gray' : undefined;
    return(
        <p style={{backgroundColor: backgroud}} >
            {props.content.get('rg') == '4' ? <span style={{color: 'yellow'}} >[房] </span> : null}
            {props.content.get('bl') != '0' ? <span style={{color: 'yellow'}}>[{props.content.get('bl')} {props.content.get('bnn')}] </span> : null}
            <span>Lv {props.content.get('level')} </span>
            <span style={{color: 'green'}}>{props.content.get('nn')}: </span>
            <span>{props.content.get('txt')} </span>
        </p>
        // <p>{props.content.get('txt')}</p>
    );
}