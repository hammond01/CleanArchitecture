import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { message } from 'antd';
import React from 'react';
import { supplierApi } from '@/services/api';

interface Props { visible: boolean; onCancel: () => void; onSuccess: () => void }

const CreateSupplierForm: React.FC<Props> = ({ visible, onCancel, onSuccess }) => {
  const [messageApi, contextHolder] = message.useMessage();
  return (
    <>
      {contextHolder}
      <ModalForm
        title={<FormattedMessage id="pages.supplier.new" defaultMessage="New Supplier" />}
        width="420px"
        visible={visible}
        onVisibleChange={(v) => !v && onCancel()}
        onFinish={async (values) => {
          try {
            const res = await supplierApi.create(values as any);
            if (res && res.statusCode === 201) { messageApi.success('Supplier created'); onSuccess(); return true; }
            messageApi.error(res?.message || 'Failed to create supplier'); return false;
          } catch { messageApi.error('Failed to create supplier'); return false; }
        }}
      >
        <ProFormText name="companyName" label="Company" rules={[{ required: true }]} />
        <ProFormText name="contactName" label="Contact" />
        <ProFormText name="phone" label="Phone" />
        <ProFormText name="country" label="Country" />
      </ModalForm>
    </>
  );
};

export default CreateSupplierForm;
