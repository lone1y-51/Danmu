import React from 'react';
export default function SingleGift(props: {content: Map<any, any>, gift: Map<any, any>}){
    const {content, gift} = props;
    return(
        (gift && gift.get(content.get("gfid")) !== undefined) ? 
            <p>{`Lv `+ content.get('level') + ` ` + content.get('nn') + `: ` + gift.get(content.get('gfid')) + `X` + content.get('gfcnt') + ` ` + content.get('hits') + `连击`}</p>
            : null
    );
}