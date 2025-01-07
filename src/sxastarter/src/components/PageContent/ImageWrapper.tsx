import { Image as JssImage } from '@sitecore-jss/sitecore-jss-nextjs'
import React from 'react'
import { CarouselItemProps } from './Carousel'
import { PromoCtaProps } from './PromoCta'
import Image from 'next/image'

type ImageWrapperProps = {
    item: CarouselItemProps | PromoCtaProps,
    isPageEditing: boolean,
    editClass: string,
    viewClass: string
}

export default function ImageWrapper({item, isPageEditing, editClass, viewClass}: ImageWrapperProps) {
  const mediaUrl = "https://financial.sitecoreedge.online"  + removeHostPartFromUrl(item.fields?.Image?.value?.src?.toString() + "")
  return (
      <>{ isPageEditing ?   <JssImage
                field={item.fields.Image}
                className={`${editClass}`}
                height={' '}
              ></JssImage> : <Image width={1920} height={720} alt='image' src={mediaUrl} className={`${viewClass}`}></Image>
      }</>
  )
}

const removeHostPartFromUrl = (targetUrl: string) => {
    const url = new URL(targetUrl);
    return targetUrl.replace(url.origin, '');
  }