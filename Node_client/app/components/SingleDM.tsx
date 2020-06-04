import React from 'react';
export default function SingleDM(props: {content: Map<any, any>}){
    return(
        <div>{props.content.get('txt')}</div>
    );
}