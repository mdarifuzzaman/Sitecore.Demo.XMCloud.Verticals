import { Image } from '@sitecore-jss/sitecore-jss-nextjs'
import React from 'react'
import { CarouselItemProps } from './Carousel'
import { PromoCtaProps } from './PromoCta'

type ImageWrapperProps = {
    item: CarouselItemProps | PromoCtaProps,
    isPageEditing: boolean,
    editClass: string,
    viewClass?: string
}

export default function ImageWrapper({item, editClass}: ImageWrapperProps) {
  const mediaUrl = "https://financial.sitecoreedge.online"  + removeHostPartFromUrl(item.fields?.Image?.value?.src?.toString() + "")
  const modifiedField = {
    ...item.fields.Image,
    value: {
      ...item.fields.Image.value,
      src: mediaUrl, // Replace original URL with transformed URL
    },
 };
  return (
      <Image
                field={modifiedField}
                className={`${editClass}`}
              ></Image> 
      
  )
}

const removeHostPartFromUrl = (targetUrl: string) => {
    const url = new URL(targetUrl);
    return targetUrl.replace(url.origin, '');
  }